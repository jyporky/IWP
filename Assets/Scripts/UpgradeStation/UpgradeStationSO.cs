using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeStationSO", menuName = "ScriptableObject/UpgradeStation")]
public class UpgradeStationSO : ScriptableObject
{
    [Header("Upgrade Item List References")]
    public List<StatsUpgrade> statsUpgradeList;

    [Header("Cards Upgrade List Reference")]
    public List<UpgradeCardSO> upgradeCardList;
}
