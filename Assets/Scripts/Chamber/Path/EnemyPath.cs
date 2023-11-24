using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPath : PathInfo
{
    private EnemySO enemySO;

    /// <summary>
    /// Set the enemySo reference as well as update the path info display
    /// </summary>
    /// <param name="enemySOReference"></param>
    public void SetEnemy(EnemySO enemySOReference)
    {
        enemySO = enemySOReference;
        SetPath(enemySO.enemyName, enemySO.enemyDescription, enemySO.enemySprite);
    }

    public override void PathSelected()
    {
        EnemyManager.GetInstance().SetEnemySO(enemySO);
        ChamberManager.GetInstance().InteractWithPath(PathType.ENEMY);
    }
}
