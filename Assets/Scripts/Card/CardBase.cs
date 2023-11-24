using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] Image cardSprite;
    [SerializeField] TextMeshProUGUI cardDescription;
    protected CardSO card;

    public virtual void UpdateCardDetails(CardSO cardInfo)
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
