using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorEnemy : EnemyBase
{
    bool abilityTriggered;
    [SerializeField] CardSO berserkCardSO;

    protected override void Start()
    {
        base.Start();
        abilityTriggered = false;
    }

    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);

        if (!abilityTriggered && currentHP <= maxHP/2)
        {
            abilityTriggered = true;
            AddCardsToPile(ModifyByAmountOfCardsType.BY_CARDS_IN_HANDS, berserkCardSO);
        }
    }
}
