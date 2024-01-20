using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusEnemy : EnemyBase
{
    [SerializeField] Keyword virusShieldReference;
    public override void StartTurn()
    {
        base.StartTurn();
        AddStatusEffect(virusShieldReference);
    }
}
