using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [Header("Card UI Reference")]
    [SerializeField] protected TextMeshProUGUI cardCost;
    [SerializeField] Image cardTypeIcon;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] Image cardSprite;
    [SerializeField] TextMeshProUGUI cardDescription;

    [Header("Card Text Color")]
    [SerializeField] Color32 nonUpgradedColor = Color.white;
    [SerializeField] Color32 upgradedColor = Color.green;

    [Header("Card Cost Color")]
    [SerializeField] Color32 playableColor = Color.white;
    [SerializeField] Color32 notPlayableColor = Color.red;
    protected CardSO card;

    public virtual void UpdateCardDetails(CardSO cardInfo)
    {
        card = cardInfo;
        cardName.text = card.cardName;
        cardSprite.sprite = card.cardSprite;
        cardDescription.text = card.cardDescription;
        cardCost.text = card.cardCost.ToString();

        switch (card.isUpgraded)
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

        cardTypeIcon.sprite = AssetManager.GetInstance().GetCardSprite(card.cardType);
    }

    /// <summary>
    /// Update the cost color as well as whether it can be played according to the amount of nexus core the player owned.
    /// </summary>
    public virtual void UpdatePlayableState(int entityNexusAmt)
    {
        if (entityNexusAmt < card.cardCost)
            cardCost.color = notPlayableColor;
        else
            cardCost.color = playableColor;
    }

    public CardSO GetCardSO()
    {
        return card;
    }
}
