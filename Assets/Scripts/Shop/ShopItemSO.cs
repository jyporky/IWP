using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItemSO", menuName = "ScriptableObject/ShopItem")]
public class ShopItemSO : ScriptableObject
{
    public CardSO cardSOItem;
    public int gearPartCost;
}
