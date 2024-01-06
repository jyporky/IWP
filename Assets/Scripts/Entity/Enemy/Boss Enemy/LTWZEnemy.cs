using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LTWZEnemy : EnemyBase
{
    public override void StartTurn()
    {
        base.StartTurn();
        int totalAmt = 0;
        totalAmt += GetPowerOfAmount(cardsInDeckList);
        totalAmt += GetPowerOfAmount(cardsInHandList);
        totalAmt += GetPowerOfAmount(cardsInDiscardList);
        int totalDmg = totalAmt / 2;
        CardManager.GetInstance().GetPlayerReference().ChangeHealth(-totalDmg);
    }

    /// <summary>
    /// Return the total count of cards that contain "Power Of" in the list.
    /// </summary>
    int GetPowerOfAmount(List<CardSO> cardListRef)
    {
        int amt = 0;
        foreach (CardSO card in cardListRef)
        {
            if (card.cardName.Contains("Power Of"))
                amt++;
        }
        return amt;
    }
}
