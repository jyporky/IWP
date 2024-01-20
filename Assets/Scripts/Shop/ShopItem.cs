using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] ShopCard shopCard;
    [SerializeField] Button viewCardButton;
    [SerializeField] Button purchaseCardButton;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Color32 purchasableColor;
    [SerializeField] Color32 notPurchasableColor;
    private ShopManagerUI shopManagerUI;
    private ShopItemSO shopItemSO;
    private bool bought;
    private bool purchasable;

    /// <summary>
    /// Update the shop item info.
    /// </summary>
    public void UpdateShopItemInfo(ShopItemSO newShopitemSO, ShopManagerUI smUI)
    {
        shopItemSO = newShopitemSO;
        costText.text = shopItemSO.gearPartCost.ToString();
        shopCard.UpdateCardDetails(shopItemSO.cardSOItem);
        bought = false;
        shopManagerUI = smUI;
        viewCardButton.onClick.AddListener(DisplayCard);
        purchaseCardButton.onClick.AddListener(PurchaseItem);
    }

    /// <summary>
    /// Check to see whether this item is purchasable depending on the amount of gear parts the player has. <br/>
    /// Ignore this if this item is already bought.
    /// </summary>
    public void UpdatePurchasable(int playerAmount)
    {
        if (bought)
            return;

        if (playerAmount >= shopItemSO.gearPartCost)
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
        if (bought)
            return;

        switch (purchasable)
        {
            case true:
                costText.color = purchasableColor;
                purchaseCardButton.interactable = true;
                break;
            case false:
                costText.color = notPurchasableColor;
                purchaseCardButton.interactable = false;
                break;
        }
    }

    /// <summary>
    /// Purchase the selected item if it is purchasable
    /// </summary>
    void PurchaseItem()
    {
        if (!purchasable || bought)
            return;

        shopManagerUI.PurchaseSelectedItem(shopItemSO);
        bought = true;
        shopCard.Bought();
        costText.text = "-";
        costText.color = purchasableColor;
        purchaseCardButton.interactable = false;
    }

    /// <summary>
    /// Display more about the card to the ShopManagerUI
    /// </summary>
    void DisplayCard()
    {
        shopManagerUI.DisplayCard(shopCard.GetCardSO());
    }
}
