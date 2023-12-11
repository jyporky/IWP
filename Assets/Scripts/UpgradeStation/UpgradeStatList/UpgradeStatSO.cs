using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeStatSO", menuName = "ScriptableObject/UpgradeStat")]
public class UpgradeStatSO : ScriptableObject
{
    public UpgradeType upgradeItemType;
    public Sprite upgradeItemIcon;
    public string upgradeItemName;
    public string upgradeItemDescription;
    public int baseCost;
    public int increaseCost;
}
