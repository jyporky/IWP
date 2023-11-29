using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UpgradeType
{
    HEALTH,
    ENERGY_POINT,
    TOTAL,
}

public class UpgradeStationManager : MonoBehaviour
{
    [System.Serializable]
    private class UpgradeAmount
    {
        public UpgradeType upgradeType;
        public int modifyValueBy;
        public int amountPurchase;
    }

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

    [SerializeField] List<UpgradeAmount> upgradeAmountList;

    /// <summary>
    /// Get the amount of upgrade made on the upgradeType parameter
    /// </summary>
    public int GetUpgradeAmount(UpgradeType whichUpgradeType)
    {
        for (int i = 0; i < upgradeAmountList.Count; i++)
        {
            if (whichUpgradeType == upgradeAmountList[i].upgradeType)
                return upgradeAmountList[i].amountPurchase;
        }
        return 0;
    }

    /// <summary>
    /// Increase the amount of upgrade made on the upgradeType parameter by 1.
    /// </summary>
    public void IncreaseUpgradeAmount(UpgradeType whichUpgradeType)
    {
        for (int i = 0; i < upgradeAmountList.Count; i++)
        {
            if (whichUpgradeType == upgradeAmountList[i].upgradeType)
            {
                upgradeAmountList[i].amountPurchase++;
                ExecuteUpgradeEffect(whichUpgradeType, upgradeAmountList[i].modifyValueBy);
                break;
            }
                
        }
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
