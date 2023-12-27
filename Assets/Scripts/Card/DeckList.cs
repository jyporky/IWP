using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckList : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] Button closeDeckList;
    [SerializeField] TextMeshProUGUI titleTextDisplay;
    [Header("Deck Card Reference")]
    [SerializeField] GameObject deckCardPrefab;
    [SerializeField] Transform deckCardListSpawnArea;
    [Header("Selected Card Reference")]
    [SerializeField] GameObject selectedCardPrefab;
    [SerializeField] GameObject outsideAreaZone;

    private Dictionary<CardSO, int> deckCardDictionary = new Dictionary<CardSO, int>();

    /// <summary>
    /// Set up the base for the deck list, the cards created is directly dependant on the list in the parameter input. <br/>
    /// the titleText represent the text being display as the title.
    /// </summary>
    public void SetupDeckList(List<CardSO> cardList, string titleText = "Deck List")
    {
        titleTextDisplay.text = titleText;
        closeDeckList.onClick.AddListener(ClosePanel);
        // Put the list in the dictionary first, to get the amount for each of the cards.
        for (int i = 0; i < cardList.Count; i++)
        {
            if (!deckCardDictionary.ContainsKey(cardList[i]))
            {
                deckCardDictionary.Add(cardList[i], 1);
            }
            else
            {
                deckCardDictionary[cardList[i]]++;
            }
        }

        // After setting up the dictionary, create the cards and slot the values in.
        foreach (var cardSO in deckCardDictionary)
        {
            GameObject newDeckCard = Instantiate(deckCardPrefab, deckCardListSpawnArea);
            newDeckCard.GetComponent<DeckCard>().UpdateCardDetails(cardSO.Key, cardSO.Value, this);
        }
        EnableDisableOutsideZone(false);
        outsideAreaZone.GetComponent<Button>().onClick.AddListener(DeSelectCard);
    }

    /// <summary>
    /// Close panel. Destroy the deck list and the cards within it.
    /// </summary>
    void ClosePanel()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Create the selectedCardSO, which display the card as well as the list of keywords definition within it.
    /// </summary>
    /// <param name="selectedCardSO"></param>
    public void DisplaySelectedCard(CardSO selectedCardSO)
    {
        GameObject newSelectedCard = Instantiate(selectedCardPrefab, outsideAreaZone.transform);
        newSelectedCard.transform.localScale = new Vector3(2, 2, 1);
        newSelectedCard.GetComponent<SelectedCard>().UpdateCardDetails(selectedCardSO);
        EnableDisableOutsideZone(true);
    }

    void DeSelectCard()
    {
        EnableDisableOutsideZone(false);
        Destroy(outsideAreaZone.GetComponentInChildren<SelectedCard>().gameObject);
    }

    void EnableDisableOutsideZone(bool toEnable)
    {
        outsideAreaZone.SetActive(toEnable);
    }
}
