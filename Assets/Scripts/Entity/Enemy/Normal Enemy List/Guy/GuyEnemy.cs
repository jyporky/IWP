using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuyEnemy : EnemyBase
{
    [SerializeField] CardSO hastePlusCard;
    public override void ReshuffleDeck()
    {
        base.ReshuffleDeck();
        AddCardsToPile(ModifyByAmountOfCardsType.BY_CARDS_IN_DRAW_PILE, hastePlusCard);
    }
}
