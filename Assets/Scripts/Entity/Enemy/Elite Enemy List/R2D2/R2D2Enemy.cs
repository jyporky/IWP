using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R2D2Enemy : EnemyBase
{
    [SerializeField] CardSO energizeCardSO;
    public override void StartTurn()
    {
        base.StartTurn();
        AddCardsToPile(ModifyByAmountOfCardsType.BY_CARDS_IN_HANDS, energizeCardSO);
    }
}
