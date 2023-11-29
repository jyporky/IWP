using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItemSO", menuName = "ScriptableObject/UpgradeItem")]
public class UpgradeItemSO : ScriptableObject
{
    public UpgradeType upgradeItemType;
    public Sprite upgradeItemIcon;
    public string upgradeItemName;
    public string upgradeItemDescription;
    public int baseCost;
    public int increaseCost;
}
