using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/Enemy")]
public class EnemySO : ScriptableObject
{
    public string enemyName;
    [TextArea]
    public string enemyDescription;
    public Sprite enemySprite;
    public int enemyHP;
    public int level;
    public int enemySP;
    public int drawCardAmt = 1;
    public List<CardSO> enemyDeck;

    [Header("Loot Drop Info")]
    public int gearPartsDrop;
    public int energyPointDrop;

    [Header("Enemy attack pattern")]
    
    /// <summary>
    /// the attack pattern that the enemy would go for. Note that cardFocus follow the same order as in the cardFocusList.
    /// </summary>
    public List<DecisionType> decisionPattern;

    /// <summary>
    /// Compromise of the cards that the enemy would prefer to play if possible.
    /// </summary>
    public List<CardSO> cardFocusList;
}
