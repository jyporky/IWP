using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicEnemy : EnemyBase
{
    [SerializeField] CardSO EmergencyRepairCardRef;
    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);

        if (healthChanged < 0)
        {
            AddCardsToPile(ModifyByAmountOfCardsType.BY_CARDS_IN_DRAW_PILE, EmergencyRepairCardRef, 1);
        }
    }
}
