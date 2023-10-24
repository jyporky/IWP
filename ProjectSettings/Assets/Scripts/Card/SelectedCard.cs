using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectedCard : MonoBehaviour
{
    [SerializeField] Image cardAttackTypeSprite;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] Image cardSprite;
    [SerializeField] TextMeshProUGUI cardDescription;
    private CardSO card;

    public void UpdateCardDetails(CardSO cardInfo)
    {
        card = cardInfo;
        cardName.text = cardInfo.cardName;
        cardSprite.sprite = cardInfo.cardSprite;
        cardDescription.text = cardInfo.cardDescription;
    }

    public CardSO GetCardSO()
    {
        return card;
    }
}
