using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EnemyCard : CardBase
{
    private CardSO cardSO;
    private bool playable;

    public override void UpdateCardDetails(CardSO cardInfo)
    {
        card = cardInfo;
        playable = false;
    }

    public override void UpdatePlayableState(int entityNexusAmt)
    {
        // Set the card to a playable state or not depending on the amount.
        if (entityNexusAmt >= card.cardCost)
        {
            playable = true;
        }
        else
        {
            playable = false;
        }
    }
}
