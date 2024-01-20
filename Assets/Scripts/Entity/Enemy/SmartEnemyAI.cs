using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use to determine the decision type of the enemy: <br/>
/// - Damage Output focuses on doing damage.<br/>
/// - Card Focus relies on specifically playing that card if possible.<br/>
/// - Increase Damage focuses on increasing owns damage if possible. <br/>
/// - Surviability focuses on healing self if possible. <br/>
/// - Draw Focus relies on drawing cards if possible. <br/>
/// </summary>
public enum DecisionType
{
    Damage_Output,
    Card_Focus,
    Increase_Damage,
    Surviability,
    Draw_Focus,
    Damage_Overtime,
}

/// <summary>
/// A class to determine the score value of the card. <br/>
/// DmgScore: ScoreValue is equal to the damage. <br/>
/// DmgModifierScore: ScoreValue is equal to any dmgModifier value such as Weaken(-)/Strength(+)/Marked(+).
/// </summary>
public class CardScore
{
    public int dmgScore;
    public int dmgModifierScore;
    public int dmgOvertimeScore;
    public int drawScore;
    public int survivalScore;
}

public static class SmartEnemyAI
{
    /// <summary>
    /// Return the best card to play in a given situation. <br/>
    /// First Iteration, only considering cardsInHand as well as the StartingEnergy.
    /// </summary>
    public static CardSO CardToPlay(List<CardSO> cardsInHand, int energyLeft, DecisionType decisionType, CardSO focusCard = null)
    {
        // Generate the list of possible plays the AI could do.
        List<CardSO> listOfDecision = new(cardsInHand);

        // If it is a focusCard, play that focus card if possible.
        if (focusCard != null && decisionType == DecisionType.Card_Focus)
        {
            foreach (CardSO card in listOfDecision)
            {
                if (card == focusCard)
                    return card;
            }
        }

        // Calculate the scoreValue, and take whichever is the highest
        float scoreValue = -Mathf.Infinity;
        CardSO bestOutcome = null;

        foreach(var card in listOfDecision)
        {
            if (energyLeft < card.cardCost)
                continue;

            CardScore cardScore = card.GetCardScore();

            switch (decisionType)
            {
                case DecisionType.Damage_Output:
                    if (cardScore.dmgScore > scoreValue)
                    {
                        scoreValue = cardScore.dmgScore;
                        bestOutcome = card;
                    }
                    break;
                case DecisionType.Draw_Focus:
                    if (cardScore.drawScore > scoreValue)
                    {
                        scoreValue = cardScore.drawScore;
                        bestOutcome = card;
                    }
                    break;
                case DecisionType.Surviability:
                    if (cardScore.survivalScore > scoreValue)
                    {
                        scoreValue = cardScore.survivalScore;
                        bestOutcome = card;
                    }
                    break;
                case DecisionType.Increase_Damage:
                    if (cardScore.dmgModifierScore > scoreValue)
                    {
                        scoreValue = cardScore.dmgModifierScore;
                        bestOutcome = card;
                    }
                    break;
                case DecisionType.Damage_Overtime:
                    if (cardScore.dmgOvertimeScore > scoreValue)
                    {
                        scoreValue = cardScore.dmgOvertimeScore;
                        bestOutcome = card;
                    }
                    break;
            }
        }

        return bestOutcome;
    }

    /// <summary>
    /// Check to see if that decision is valid. If valid, return true, else return false.
    /// </summary>
    public static bool GetValidDecision(List<CardSO> cardList, DecisionType decision, int energyRemaining, CardSO cardFocus = null)
    {
        // Check to see if the Card have a cardScore value
        foreach (CardSO card in cardList)
        {
            if (card.GetCardScore() == null)
                card.SetCardScore(CardManager.GetCardScore(card));
        }

        // Depending on the decision, check to see if that decision can be executed.
        switch (decision)
        {
            case DecisionType.Damage_Output:
                {
                    bool dmgCardExist = false;

                    foreach (CardSO card in cardList)
                    {
                        if (card.cardCost > energyRemaining)
                            continue;

                        if (card.GetCardScore().dmgScore > 0)
                            dmgCardExist = true;
                    }
                    if (!dmgCardExist)
                        return false;

                    break;
                }

            case DecisionType.Increase_Damage:
                {
                    bool dmgModifierCardExist = false;

                    foreach (CardSO card in cardList)
                    {
                        if (card.cardCost > energyRemaining)
                            continue;

                        if (card.GetCardScore().dmgModifierScore > 0)
                            dmgModifierCardExist = true;
                    }
                    if (!dmgModifierCardExist)
                        return false;

                    break;
                }

            case DecisionType.Card_Focus:
                {
                    bool cardFocusExist = false;

                    foreach (CardSO card in cardList)
                    {
                        if (card.cardCost > energyRemaining)
                            continue;

                        if (card == cardFocus)
                            cardFocusExist = true;
                    }
                    if (!cardFocusExist)
                        return false;

                    break;
                }

            case DecisionType.Draw_Focus:
                {
                    bool drawFocusExist = false;

                    foreach (CardSO card in cardList)
                    {
                        if (card.cardCost > energyRemaining)
                            continue;

                        if (card.GetCardScore().drawScore > 0)
                            drawFocusExist = true;
                    }
                    if (!drawFocusExist)
                        return false;

                    break;
                }

            case DecisionType.Surviability:
                {
                    bool surviabilityCardExist = false;

                    foreach (CardSO card in cardList)
                    {
                        if (card.cardCost > energyRemaining)
                            continue;

                        if (card.GetCardScore().survivalScore > 0)
                            surviabilityCardExist = true;
                    }
                    if (!surviabilityCardExist)
                        return false;

                    break;
                }

            case DecisionType.Damage_Overtime:
                {
                    bool dmgoverttimeCardExist = false;

                    foreach (CardSO card in cardList)
                    {
                        if (card.cardCost > energyRemaining)
                            continue;

                        if (card.GetCardScore().dmgOvertimeScore > 0)
                            dmgoverttimeCardExist = true;
                    }
                    if (!dmgoverttimeCardExist)
                        return false;

                    break;
                }
        }

        return true;
    }
}
