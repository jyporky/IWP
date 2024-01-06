using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Use to determine the decision type of the enemy: <br/>
/// - Damage Output focuses on maximizing damage using cards on your hand and in your deck.<br/>
/// - Card Focus relies on specifically playing that card and draw as many cards to shuffle that card back.<br/>
/// </summary>
public enum DecisionType
{
    Damage_Output,
    Card_Focus,
}

public static class SmartEnemyAI
{
    /// <summary>
    /// Return the list of cards to play in order, would be the best decision according to the type.
    /// </summary>
    //public static List<CardSO> MinMax(List<CardSO> cardsInHand, DecisionType decision)
    //{

    //}
}