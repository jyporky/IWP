using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainEnemy : EnemyBase
{
    int increaseNexusBy;
    protected override void Start()
    {
        increaseNexusBy = 0;
    }

    public override void ReshuffleDeck()
    {
        base.ReshuffleDeck();
        increaseNexusBy++;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        currentNexusCoreAmount += increaseNexusBy;
    }
}
