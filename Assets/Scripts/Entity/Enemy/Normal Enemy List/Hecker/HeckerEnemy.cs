using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeckerEnemy : EnemyBase
{
    public override void PlayCard(CardSO cardPlayed)
    {
        base.PlayCard(cardPlayed);

        if (cardPlayed.cardType == CardType.Trojan)
        {
            DrawCardFromDeck(1
                );
        }
    }
}
