using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class DeckCard : CardBase
{
    [SerializeField] Button self;
    [Header("Amount")]
    [SerializeField] GameObject amountDisplay;
    [SerializeField] TextMeshProUGUI amountText;
    private DeckList deckList;

    public void UpdateCardDetails(CardSO cardInfo, int amount, DeckList deckListReference)
    {
        UpdateCardDetails(cardInfo);
        deckList = deckListReference;
        amountDisplay.SetActive(false);

        if (amount > 1)
        {
            amountText.text = "x" + amount.ToString();
            amountDisplay.SetActive(true);
        }

        self.onClick.AddListener(SelectCard);
    }

    /// <summary>
    /// Send the cardSO to the deckList Manager to be displayed.
    /// </summary>
    void SelectCard()
    {
        deckList.DisplaySelectedCard(card);
    }
}
