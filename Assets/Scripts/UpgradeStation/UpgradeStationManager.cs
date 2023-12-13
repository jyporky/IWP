using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    HEALTH,
    ENERGY_POINT,
    TOTAL,
}

[System.Serializable]
public class StatsUpgrade
{
    public UpgradeType upgradeType;
    public int modifyValueBy;
    public int amountPurchase;
}

public class UpgradeStationManager : MonoBehaviour
{
    private static UpgradeStationManager instance;
    public static UpgradeStationManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    [SerializeField] UpgradeStationSO upgradeStationSO;
    private List<StatsUpgrade> statsUpgradeList;
    private List<UpgradeCardSO> cardUpgradesList;

    private void Start()
    {
        statsUpgradeList = new List<StatsUpgrade>();
        foreach (StatsUpgrade upgrade in upgradeStationSO.statsUpgradeList)
        {
            StatsUpgrade newStatUpgrade = new StatsUpgrade();
            newStatUpgrade.upgradeType = upgrade.upgradeType;
            newStatUpgrade.modifyValueBy = upgrade.modifyValueBy;
            newStatUpgrade.amountPurchase = upgrade.amountPurchase;
            statsUpgradeList.Add(newStatUpgrade);
        }
        cardUpgradesList = new(upgradeStationSO.upgradeCardList);
    }

    /// <summary>
    /// Get the list of upgradable cards given a list of cardSO. <br/>
    /// The card can be upgraded if an upgradecardSO exist within the cardUpgradesList.
    /// </summary>
    /// <returns></returns>
    public List<CardSO> GetUpgradableCards(List<CardSO> listToCheck)
    {
        List<CardSO> listToReturn = new List<CardSO>();
        foreach(CardSO card in listToCheck)
        {
            for (int i = 0; i < cardUpgradesList.Count; i++)
            {
                if (cardUpgradesList[i].baseCard == card)
                {
                    listToReturn.Add(card);
                    break;
                }
            }
        }
        return listToReturn;
    }

    /// <summary>
    /// Get the amount of upgrade made on the upgradeType parameter
    /// </summary>
    public int GetUpgradeAmount(UpgradeType whichUpgradeType)
    {
        for (int i = 0; i < statsUpgradeList.Count; i++)
        {
            if (whichUpgradeType == statsUpgradeList[i].upgradeType)
                return statsUpgradeList[i].amountPurchase;
        }
        return 0;
    }

    /// <summary>
    /// Increase the amount of upgrade made on the upgradeType parameter by 1.
    /// </summary>
    public void IncreaseUpgradeAmount(UpgradeType whichUpgradeType)
    {
        for (int i = 0; i < statsUpgradeList.Count; i++)
        {
            if (whichUpgradeType == statsUpgradeList[i].upgradeType)
            {
                statsUpgradeList[i].amountPurchase++;
                ExecuteUpgradeEffect(whichUpgradeType, statsUpgradeList[i].modifyValueBy);
                break;
            }
        }
    }

    /// <summary>
    /// Get the upgradeSO according to the upgradable card.
    /// </summary>
    public UpgradeCardSO GetUpgradeCardSO(CardSO card)
    {
        for (int i = 0; i < cardUpgradesList.Count; i++)
        {
            if (card == cardUpgradesList[i].baseCard)
            {
                return cardUpgradesList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Execute the card effect according to what upgrade type.
    /// </summary>
    void ExecuteUpgradeEffect(UpgradeType upgradeType, int modifyValueBy)
    {
        switch (upgradeType)
        {
            case UpgradeType.HEALTH:
                PlayerManager.GetInstance().ModifyIncreaseHP(modifyValueBy);
                break;
            case UpgradeType.ENERGY_POINT:
                PlayerManager.GetInstance().ModifyIncreaseEP(modifyValueBy);
                break;
        }
    }
}
