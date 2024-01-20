
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cards", menuName = "ScriptableObject/Card")]
public class CardSO : ScriptableObject
{
    public Sprite cardSprite;
    public CardType cardType;
    public int cardCost = 1;
    public string cardName;
    [TextArea]
    public string cardDescription;
    public bool isUpgraded;
    public List<Keyword> keywordsList;

    private CardScore cardScore;

    public CardScore GetCardScore()
    {
        return cardScore;
    }

    public void SetCardScore(CardScore newCardScore)
    {
        cardScore = newCardScore;
    }
}
