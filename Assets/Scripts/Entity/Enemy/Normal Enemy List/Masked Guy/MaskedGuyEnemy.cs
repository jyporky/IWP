using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskedGuyEnemy : EnemyBase
{
    [SerializeField] CardSO swipeMasterCardRef;

    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);
        if (currentHP < 10)
        {
            AddCardsToPile(ModifyByAmountOfCardsType.BY_CARDS_IN_HANDS, swipeMasterCardRef);
        }    
    }
}
