using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [SerializeField] protected Sprite virusIcon;
    [SerializeField] protected Sprite wormIcon;
    [SerializeField] protected Sprite trojanIcon;
    [SerializeField] Image cardTypeIcon;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] Image cardSprite;
    [SerializeField] TextMeshProUGUI cardDescription;
    [SerializeField] Color32 nonUpgradedColor = Color.white;
    [SerializeField] Color32 upgradedColor = Color.green;
    protected CardSO card;

    public virtual void UpdateCardDetails(CardSO cardInfo)
    {
        card = cardInfo;
        cardName.text = cardInfo.cardName;
        cardSprite.sprite = cardInfo.cardSprite;
        cardDescription.text = cardInfo.cardDescription;
        switch (cardInfo.isUpgraded)
        {
            case true:
                cardName.color = upgradedColor;
                cardDescription.color = upgradedColor;
                break;
            case false:
                cardName.color = nonUpgradedColor;
                cardDescription.color = nonUpgradedColor;
                break;
        }

        switch (card.cardType)
        {
            case CardType.Virus:
                cardTypeIcon.sprite = virusIcon;
                break;
            case CardType.Worm:
                cardTypeIcon.sprite = wormIcon;
                break;
            case CardType.Trojan:
                cardTypeIcon.sprite = trojanIcon;
                break;
        }
    }

    public CardSO GetCardSO()
    {
        return card;
    }
}
