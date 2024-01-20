using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class EnemyCard : CardBase
{
    private CardSO cardSO;

    public override void UpdateCardDetails(CardSO cardInfo)
    {
        card = cardInfo;
    }

    public override void UpdatePlayableState(int entityNexusAmt)
    {

    }
}
