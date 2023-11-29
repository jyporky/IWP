using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStationManagerUI : MonoBehaviour
{
    [System.Serializable]
    private class UpgradeItemType
    {
        public UpgradeItemSO upgradeItemSO;
        public UpgradeType upgradeType;
    }

    [SerializeField] List<UpgradeItemType> upgradeItemList;
    [SerializeField] GameObject upgradeItemPrefab;
    [SerializeField] Transform upgradeItemSpawnArea;
    [SerializeField] Button closeUpgradeStationButton;

    private void Start()
    {
        closeUpgradeStationButton.onClick.AddListener(CloseUpgradeStation);

        for (int i = 0; i < upgradeItemList.Count; i++)
        {
            GameObject newUpgradeItem = Instantiate(upgradeItemPrefab, upgradeItemSpawnArea);
            newUpgradeItem.GetComponent<UpgradeItem>().SetUpgradeItemSO(upgradeItemList[i].upgradeItemSO, upgradeItemList[i].upgradeType, this);
        }
    }

    /// <summary>
    /// Purchase the selected item.
    /// </summary>
    public void PurchaseSelectedItem(UpgradeItem upgradeItem)
    {
        UpgradeItemSO upgradeItemSO = upgradeItem.GetUpgradeItemSO();
        UpgradeStationManager.GetInstance().IncreaseUpgradeAmount(upgradeItemSO.upgradeItemType);
        PlayerManager.GetInstance().ChangeCurrentGearAmount(-upgradeItem.GetGearCost());
        GameplayManager.GetInstance().UpdatePlayerStatsDisplay();
    }

    /// <summary>
    /// Close the shop and destroy this gameobject.
    /// </summary>
    void CloseUpgradeStation()
    {
        UITransition.GetInstance().BeginTransition(result =>
        {
            ChamberManager.GetInstance().ClearRoom();
            Destroy(gameObject);
        });
    }
}
