using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManagerUI : MonoBehaviour
{
    [Header("Shop Item Spawn Area")]
    [SerializeField] GameObject shopItemPrefab;
    [SerializeField] Transform shopItemSpawnArea;

    [Header("Shop UI Reference")]
    [SerializeField] Button refreshShopButton;
    [SerializeField] TextMeshProUGUI refreshShopCostText;
    [SerializeField] Button leaveShopButton;

    [Header("Misc")]
    [SerializeField] int itemStockAmount;

    private List<ShopItemSO> possibleShopItemList = new List<ShopItemSO>();
    private int refreshShopCost;
    private PlayerManager playerManager;

    private void Start()
    {
        List<ShopItemSO> loadedShopItemList = ShopManager.GetInstance().GetShopItemList();
        for (int i = 0; i < loadedShopItemList.Count; i++)
        {
            possibleShopItemList.Add(loadedShopItemList[i]);
        }
        playerManager = PlayerManager.GetInstance();
        refreshShopCost = 0;
        refreshShopButton.onClick.AddListener(RefreshShop);
        leaveShopButton.onClick.AddListener(CloseShop);
        RefreshShop();
    }

    /// <summary>
    /// Restock the shop by removing the previous shopItem and adding a new one. <br/>
    /// Increase the refreshShop cost by 1.
    /// </summary>
    void RestockShop()
    {
        int playerGearPartsAmount = playerManager.GetGearPartsAmount();

        ShopItem[] oldShopItemList = shopItemSpawnArea.gameObject.GetComponentsInChildren<ShopItem>();
        for (int i = oldShopItemList.Length - 1; i >= 0; i--)
        {
            Destroy(oldShopItemList[i].gameObject);
        }

        for (int i = 0; i < itemStockAmount; i++)
        {
            GameObject newShopItem = Instantiate(shopItemPrefab, shopItemSpawnArea);
            newShopItem.GetComponent<ShopItem>().UpdateShopItemInfo(GetRandomShopItemSO(), this);
            newShopItem.GetComponent<ShopItem>().UpdatePurchasable(playerGearPartsAmount);
        }
    }

    /// <summary>
    /// Purchase the parameter item. Update the purchasable state afterwards.
    /// </summary>
    public void PurchaseSelectedItem(ShopItemSO selectedShopItem)
    {
        playerManager.ChangeCurrentGearAmount(-selectedShopItem.gearPartCost);
        playerManager.AddToCardList(selectedShopItem.cardSOItem);
        UpdateItemPurchasable();
        UpdateRefreshShopInteractable();
    }

    /// <summary>
    /// When player amount change, update the purchasable state of the shop item.
    /// </summary>
    void UpdateItemPurchasable()
    {
        int playerGearPartsAmount = playerManager.GetGearPartsAmount();

        ShopItem[] shopItemList = shopItemSpawnArea.gameObject.GetComponentsInChildren<ShopItem>();

        for (int i = 0; i < shopItemList.Length; i++)
        {
            shopItemList[i].GetComponent<ShopItem>().UpdatePurchasable(playerGearPartsAmount);
        }
    }

    /// <summary>
    /// Update the interactable of the refresh shop button according to the amount of gear parts the player owned. <br/>
    /// Also Update the gear amount display.
    /// </summary>
    void UpdateRefreshShopInteractable()
    {
        int playerGearPartsAmount = playerManager.GetGearPartsAmount();

        if (playerGearPartsAmount >= refreshShopCost)
        {
            refreshShopButton.interactable = true;
            refreshShopCostText.color = Color.white;
        }
        else
        {
            refreshShopButton.interactable = false;
            refreshShopCostText.color = Color.red;
        }

        refreshShopCostText.text = refreshShopCost.ToString();
        GameplayManager.GetInstance().UpdatePlayerStatsDisplay();
    }

    /// <summary>
    /// Refresh and restorck the shop
    /// </summary>
    void RefreshShop()
    {
        playerManager.ChangeCurrentGearAmount(-refreshShopCost);
        refreshShopCost += 1;
        UpdateRefreshShopInteractable();
        RestockShop();
    }

    /// <summary>
    /// Get a random shopItemSO from the avaliable shopItem list
    /// </summary>
    /// <returns></returns>
    ShopItemSO GetRandomShopItemSO()
    {
        int index = Random.Range(0, possibleShopItemList.Count);
        return possibleShopItemList[index];
    }

    /// <summary>
    /// Close the shop and destroy this gameobject.
    /// </summary>
    void CloseShop()
    {
        UITransition.GetInstance().BeginTransition(result =>
        {
            ChamberManager.GetInstance().ClearRoom();
            Destroy(gameObject);
        });
    }
}
