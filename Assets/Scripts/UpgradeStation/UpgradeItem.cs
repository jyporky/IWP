using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItem : MonoBehaviour
{
    [SerializeField] Button self;
    [SerializeField] TextMeshProUGUI upgradeItemName;
    [SerializeField] Image upgradeItemIcon;
    [SerializeField] TextMeshProUGUI upgradeItemDescription;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI costAmountText;
    [SerializeField] Color32 purchasableColor;
    [SerializeField] Color32 notPurchasableColor;

    private UpgradeItemSO upgradeItemSO;
    private int upgradeCost;
    private bool purchasable;
    private UpgradeStationManagerUI upgradeStationManagerUI;

    /// <summary>
    /// Set the upgradeItemSO into this script. Update the upgrade item display.
    /// </summary>
    public void SetUpgradeItemSO(UpgradeItemSO upgradeItemSOReference, UpgradeType upgradeTypeReference, UpgradeStationManagerUI upgradeStationManagerRef)
    {
        upgradeStationManagerUI = upgradeStationManagerRef;
        self.onClick.AddListener(PurchaseItem);
        upgradeItemSO = upgradeItemSOReference;
        upgradeItemName.text = upgradeItemSO.upgradeItemName;
        upgradeItemIcon.sprite = upgradeItemSO.upgradeItemIcon;
        upgradeItemDescription.text = upgradeItemSO.upgradeItemDescription;
        UpdateLevelAndCost();
    }

    /// <summary>
    /// Set the level and upgrade cost dislay. Store the new cost within this script.
    /// </summary>
    void UpdateLevelAndCost()
    {
        int level = UpgradeStationManager.GetInstance().GetUpgradeAmount(upgradeItemSO.upgradeItemType);
        levelText.text = "Lvl: " + level.ToString();
        upgradeCost = upgradeItemSO.baseCost + (upgradeItemSO.increaseCost * level);
        costAmountText.text = upgradeCost.ToString();

        UpdatePurchasable(PlayerManager.GetInstance().GetGearPartsAmount());
    }

    /// <summary>
    /// Check to see whether this item is purchasable depending on the amount of gear parts the player has. <br/>
    /// Update the level and cost of the item.
    /// </summary>
    void UpdatePurchasable(int playerAmount)
    {
        if (playerAmount >= upgradeCost)
        {
            purchasable = true;
        }
        else
        {
            purchasable = false;
        }

        UpdatePurchasableButton();
    }

    /// <summary>
    /// Update the state of the button and the color of the text.
    /// </summary>
    void UpdatePurchasableButton()
    {
        switch (purchasable)
        {
            case true:
                costAmountText.color = purchasableColor;
                break;
            case false:
                costAmountText.color = notPurchasableColor;
                break;
        }
    }

    /// <summary>
    /// Purchase the selected item if it is purchasable
    /// </summary>
    void PurchaseItem()
    {
        if (!purchasable)
            return;

        upgradeStationManagerUI.PurchaseSelectedItem(this);
        UpdateLevelAndCost();
    }

    /// <summary>
    /// Get the upgradeItemSO stored in this script.
    /// </summary>
    public UpgradeItemSO GetUpgradeItemSO()
    {
        return upgradeItemSO;
    }

    /// <summary>
    /// Get the gear cost for this item.
    /// </summary>
    public int GetGearCost()
    {
        return upgradeCost;
    }
}
