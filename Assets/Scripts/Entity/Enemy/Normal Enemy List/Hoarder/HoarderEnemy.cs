using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoarderEnemy : EnemyBase
{
    public override void StartTurn()
    {
        base.StartTurn();
        currentNexusCoreAmount += 1;
        DrawCardFromDeck(1);
        cm.UpdateNexusCoreDisplay(this, currentNexusCoreAmount, 3);
    }
}
