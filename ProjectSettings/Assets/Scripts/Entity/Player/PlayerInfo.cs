using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    /// <summary>
    ///  Store the list of cards player owned
    /// </summary>
    List<CardSO> listOfCards;
    /// <summary>
    /// Store the list of cards the player has in their deck
    /// </summary>
    List<CardSO> listOfCardsInDeck;
    /// <summary>
    /// The base HP of the player
    /// </summary>
    int baseHp; 
    /// <summary>
    /// The base Shield Points the player has
    /// </summary>
    int baseSP;
    /// <summary>
    /// The amount of HP increased, add this along with the baseHP for the maximum HP the player has
    /// </summary>
    int increaseHP;
    /// <summary>
    /// The amount of SP increased, add this along with the baseSP for the maximum SP the player has
    /// </summary>
    int increaseSP;

    public PlayerInfo(int BaseHp, int BaseSP)
    {
        baseHp = BaseHp;
        increaseHP = 0;
        baseSP = BaseSP;
        increaseSP = 0;
        listOfCards = new List<CardSO>();
    }

    /// <summary>
    /// Add a card to the cards owned by player
    /// </summary>
    public void AddToListOfCards(CardSO cardToAdd)
    {
        listOfCards.Add(cardToAdd);
    }

    /// <summary>
    /// Remove a card from the list of cards owned by the player
    /// </summary>
    public void RemoveFromListOfCards(CardSO cardRemoved)
    {
        for (int i = 0; i < listOfCards.Count; i++)
        {
            if (cardRemoved == listOfCards[i])
            {
                listOfCards.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Add a card to the player deck
    /// </summary>
    public List<CardSO> GetCardListFromDeck()
    {
        return listOfCards;
    }

    /// <summary>
    /// Get the MaxHP of the player
    /// </summary>
    public int GetMaxHP()
    {
        return baseHp + increaseHP;
    }

    /// <summary>
    /// Get the MaxSP of the player
    /// </summary>
    public int GetMaxSP()
    {
        return baseSP + increaseSP;
    }
}
