using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeCardSO", menuName = "ScriptableObject/UpgradeCard")]
public class UpgradeCardSO : ScriptableObject
{
    public CardSO baseCard;
    public CardSO upgradedCard;
    public int upgradeCost;
}
