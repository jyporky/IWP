using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HackAbilityInfo : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] Image abilityIcon;
    [SerializeField] TextMeshProUGUI abilityDescription;
    [SerializeField] Button abilityButton;

    private HackUpgradeUI hackUpgradeUI;
    private HackType whichHackType;

    /// <summary>
    /// Set the hack abilty info and display it.
    /// </summary>
    public void SetHackAbilityInfo(HackType hackType, int currHackLvl, HackUpgradeUI hackUpgradeUIRef)
    {
        hackUpgradeUI = hackUpgradeUIRef;
        HackingManager hm = HackingManager.GetInstance();
        abilityName.text = hm.GetHackTypeName(hackType);
        abilityIcon.sprite = hm.GetHackTypeIcon(hackType);
        HackTypeSO hackTypeSO = hm.GetHackTypeSO(hackType, currHackLvl + 1);
        abilityDescription.text = hackTypeSO.hackDescription;
        whichHackType = hackType;

        if (currHackLvl == 0)
        {
            abilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Unlock ability";
        }
        else
        {
            abilityButton.GetComponentInChildren<TextMeshProUGUI>().text = "Upgrade ability";
        }

        abilityButton.onClick.AddListener(SelectAbility);
    }

    /// <summary>
    /// Select the ability.
    /// </summary>
    private void SelectAbility()
    {
        hackUpgradeUI.SelectedHackAbility(whichHackType);
    }
}
