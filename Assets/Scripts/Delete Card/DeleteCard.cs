using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCard : CardBase
{
    [SerializeField] Button self;
    private DeleteCardManager deleteCardManager;

    public void UpdateCardDetails(CardSO cardInfo, DeleteCardManager deleteCardManagerReference)
    {
        UpdateCardDetails(cardInfo);
        deleteCardManager = deleteCardManagerReference;
        self.onClick.AddListener(SelectCard);
    }

    /// <summary>
    /// Send the cardSO to the deckList Manager to be displayed.
    /// </summary>
    void SelectCard()
    {
        deleteCardManager.SelectCard(card);
    }
}
