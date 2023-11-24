using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    /// <summary>
    /// Store the list of cards the player has in their deck
    /// </summary>
    private List<CardSO> listOfCardsInDeck;
    /// <summary>
    /// The base HP of the player
    /// </summary>
    private int baseHp;
    /// <summary>
    /// The base Energy Points the player has
    /// </summary>
    private int baseEP;
    /// <summary>
    /// The amount of HP increased, add this along with the baseHP for the maximum HP the player has
    /// </summary>
    private int increaseHP;
    /// <summary>
    /// The amount of EP increased, add this along with the baseEP for the maximum EP the player has
    /// </summary>
    int increaseEP;
    /// <summary>
    /// The current HP the player has
    /// </summary>
    private int currentHP;
    /// <summary>
    /// The current EP the player has
    /// </summary>
    private int currentEP;
    /// <summary>
    /// The amount of gears parts the player has
    /// </summary>
    private int gearPartsAmount;

    public PlayerInfo(int BaseHp, int BaseSP, int startingGearPartsAmount)
    {
        baseHp = BaseHp;
        increaseHP = 0;
        currentHP = baseHp;
        baseEP = BaseSP;
        increaseEP = 0;
        currentEP = baseEP;
        gearPartsAmount = startingGearPartsAmount;
        listOfCardsInDeck = new List<CardSO>();
    }

    /// <summary>
    /// Add a card to the cards owned by player
    /// </summary>
    public void AddToListOfCards(CardSO cardToAdd)
    {
        listOfCardsInDeck.Add(cardToAdd);
    }

    /// <summary>
    /// Remove a card from the list of cards owned by the player
    /// </summary>
    public void RemoveFromListOfCards(CardSO cardRemoved)
    {
        for (int i = 0; i < listOfCardsInDeck.Count; i++)
        {
            if (cardRemoved == listOfCardsInDeck[i])
            {
                listOfCardsInDeck.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Add a card to the player deck
    /// </summary>
    public List<CardSO> GetCardListFromDeck()
    {
        return listOfCardsInDeck;
    }

    /// <summary>
    /// Get the MaxHP of the player
    /// </summary>
    public int GetMaxHP()
    {
        return baseHp + increaseHP;
    }

    /// <summary>
    /// Get the Current HP of the player
    /// </summary>
    /// <returns></returns>
    public int GetCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// Set the current HP of the player
    /// </summary>
    public void SetCurrentHP(int newHPValue)
    {
        currentHP = newHPValue;
    }

    /// <summary>
    /// Get the MaxSP of the player
    /// </summary>
    public int GetMaxEP()
    {
        return baseEP + increaseEP;
    }

    /// <summary>
    /// Get the Current EP of the player
    /// </summary>
    /// <returns></returns>
    public int GetCurrentEP()
    {
        return currentEP;
    }

    /// <summary>
    /// Set the current EP of the player
    /// </summary>
    public void SetCurrentEP(int newEPValue)
    {
        currentEP = newEPValue;
    }

    /// <summary>
    /// Get the amount of gear parts the player owned.
    /// </summary>
    /// <returns></returns>
    public int GetGearPartsAmount()
    {
        return gearPartsAmount;
    }

    /// <summary>
    /// Change the number of gear parts the player has. <br/>
    /// Use -ve to represent a subtraction. Cannot go below 0.
    /// </summary>
    public void ChangeGearPartsAmount(int gearPartsChange)
    {
        gearPartsAmount += gearPartsChange;
        if (gearPartsAmount < 0)
            gearPartsAmount = 0;
    }
}
