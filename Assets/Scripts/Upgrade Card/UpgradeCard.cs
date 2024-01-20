using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : CardBase
{
    [SerializeField] Button self;
    private UpgradeCardManager upgradeCardManagerUIReference;

    public void UpdateCardDetails(CardSO cardInfo, UpgradeCardManager UpgradeCardManagerUIReference)
    {
        UpdateCardDetails(cardInfo);
        upgradeCardManagerUIReference = UpgradeCardManagerUIReference;
        self.onClick.AddListener(SelectCard);
    }

    /// <summary>
    /// Send the cardSO to the deckList Manager to be displayed.
    /// </summary>
    void SelectCard()
    {
        upgradeCardManagerUIReference.SelectCard(card);
    }
}
