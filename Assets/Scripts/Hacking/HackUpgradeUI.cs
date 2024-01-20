using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HackUpgradeUI : MonoBehaviour
{
    [SerializeField] GameObject hackAbilityPrefab;
    [SerializeField] TextMeshProUGUI byWhoText;
    [SerializeField] Transform hackAbilityList;

    private PlayerManager pm;

    private void Start()
    {
        pm = PlayerManager.GetInstance();
    }

    /// <summary>
    /// Display the list of possible ability player can choose to unlock/upgrade
    /// </summary>
    public void SetHackAbility(bool isBoss)
    {
        switch (isBoss)
        {
            case false:
                byWhoText.text = "Elite Enemy";
                break;
            case true:
                byWhoText.text = "Boss Enemy";
                break;
        }

        HackAbilityInfo[] oldHackABilityInfoList = hackAbilityList.GetComponentsInChildren<HackAbilityInfo>();

        for (int i = oldHackABilityInfoList.Length - 1; i >= 0; i--)
        {
            Destroy(oldHackABilityInfoList[i].gameObject);
        }

        int counter = 3;

        for (int i = 0; i < (int)HackType.Total; i++)
        {
            if (counter == 0)
                break;

            int playerCurrentHackLvl = pm.GetHackTypeMaxLevel((HackType)i);

            if (playerCurrentHackLvl < 3)
            {
                HackAbilityInfo hackAbilityInfo = Instantiate(hackAbilityPrefab, hackAbilityList).GetComponent<HackAbilityInfo>();
                hackAbilityInfo.SetHackAbilityInfo((HackType)i, playerCurrentHackLvl, this);
                counter--;
            }
        }
    }

    /// <summary>
    /// Increase the level of the selected hack ability by one.
    /// </summary>
    public void SelectedHackAbility(HackType whichHackAbility)
    {
        bool playerHaveAbility = pm.CheckIfHackTypeExist(whichHackAbility);

        if (playerHaveAbility)
        {
            pm.IncreaseHackTypeLevel(whichHackAbility);
        }
        else
        {
            pm.AddNewHackType(whichHackAbility);
        }

        gameObject.SetActive(false);
    }
}
