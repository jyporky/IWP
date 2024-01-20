using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeStationManagerUI : MonoBehaviour
{
    [System.Serializable]
    private class UpgradeItemType
    {
        public UpgradeStatSO upgradeItemSO;
        public UpgradeType upgradeType;
    }

    [Header("Upgrading Stats References")]
    [SerializeField] List<UpgradeItemType> upgradeStatList;
    [SerializeField] GameObject upgradeStatPrefab;
    [SerializeField] Transform upgradeStatSpawnArea;
    [SerializeField] Button openUpgradeCardButton;
    [SerializeField] GameObject upgradeCardUIPrefab;

    [Header("Misc")]
    [SerializeField] Button closeUpgradeStationButton;
    private PlayerManager pm;
    private GameplayManager gm;
    private UpgradeStationManager usm;
    private int upgradeCostAmount;
    private Transform UISpawnArea;

    public delegate void OnPurchase();
    /// <summary>
    /// Delegate function that invokes when an upgrade is purchase, stats and cards.
    /// </summary>
    public OnPurchase onPurchase;

    private void Start()
    {
        closeUpgradeStationButton.onClick.AddListener(CloseUpgradeStation);

        for (int i = 0; i < upgradeStatList.Count; i++)
        {
            GameObject newUpgradeItem = Instantiate(upgradeStatPrefab, upgradeStatSpawnArea);
            newUpgradeItem.GetComponent<UpgradeStat>().SetUpgradeItemSO(upgradeStatList[i].upgradeItemSO, upgradeStatList[i].upgradeType, this);
        }

        openUpgradeCardButton.onClick.AddListener(OpenUpgradeCardPanel);

        pm = PlayerManager.GetInstance();
        gm = GameplayManager.GetInstance();
        usm = UpgradeStationManager.GetInstance();
        UISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
    }

    /// <summary>
    /// Load to the upgrade station and update its UI.
    /// </summary>
    public void LoadToUpgradeStation()
    {
        UpgradeStat[] upgradeStatRefList = upgradeStatSpawnArea.GetComponentsInChildren<UpgradeStat>();
        foreach (UpgradeStat upgradeStat in upgradeStatRefList)
        {
            upgradeStat.UpdateLevelAndCost();
        }
    }

    /// <summary>
    /// Purchase the selected item.
    /// </summary>
    public void PurchaseSelectedItem(UpgradeStat upgradeItem)
    {
        UpgradeStatSO upgradeItemSO = upgradeItem.GetUpgradeItemSO();
        usm.IncreaseUpgradeAmount(upgradeItemSO.upgradeItemType);
        pm.ChangeCurrentGearAmount(-upgradeItem.GetGearCost());
        gm.UpdatePlayerStatsDisplay();
        onPurchase?.Invoke();
    }

    /// <summary>
    /// Close the shop and destroy this gameobject.
    /// </summary>
    void CloseUpgradeStation()
    {
        UITransition.GetInstance().BeginTransition(result =>
        {
            ChamberManager.GetInstance().ClearRoom();
        });
    }

    /// <summary>
    /// Open the upgrade card panel.
    /// </summary>
    void OpenUpgradeCardPanel()
    {
        UpgradeCardManager upgradeCardManager = Instantiate(upgradeCardUIPrefab, UISpawnArea).GetComponent<UpgradeCardManager>();
        upgradeCardManager.SetPurchaseRef(this);
    }
}
