using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeReaperEnemy : EnemyBase
{
    [SerializeField] CardSO plungePlusCardRef;

    public override void ReshuffleDeck()
    {
        base.ReshuffleDeck();
        AddCardsToPile(ModifyByAmountOfCardsType.BY_CARDS_IN_DRAW_PILE, plungePlusCardRef);
    }
}
