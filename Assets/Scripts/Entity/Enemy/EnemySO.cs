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
}
