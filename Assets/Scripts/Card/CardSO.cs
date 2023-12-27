
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cards", menuName = "ScriptableObject/Card")]
public class CardSO : ScriptableObject
{
    public Sprite cardSprite;
    public CardType cardType;
    public int cardCost;
    public string cardName;
    [TextArea]
    public string cardDescription;
    public bool isUpgraded;
    public List<Keyword> keywordsList;
}
