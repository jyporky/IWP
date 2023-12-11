using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : CardBase
{
    [SerializeField] Button self;
    private UpgradeStationManagerUI upgradeStationManagerUIReference;

    public void UpdateCardDetails(CardSO cardInfo, UpgradeStationManagerUI UpgradeStationManagerUIReference)
    {
        UpdateCardDetails(cardInfo);
        upgradeStationManagerUIReference = UpgradeStationManagerUIReference;
        self.onClick.AddListener(SelectCard);
    }

    /// <summary>
    /// Send the cardSO to the deckList Manager to be displayed.
    /// </summary>
    void SelectCard()
    {
        upgradeStationManagerUIReference.SelectCard(card);
    }
}
