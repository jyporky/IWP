using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemy : EnemyBase
{
    public override void StartTurn()
    {
        base.StartTurn();
        ChangeHealth(1);
    }
}
