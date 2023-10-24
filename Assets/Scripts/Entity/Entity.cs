using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Entity : MonoBehaviour
{
    protected int maxHP;
    protected int currentHP;
    protected int maxSP;
    protected int currentSP;
    private List<GameObject> cardList = new List<GameObject>();
    protected List<CardSO> cardsInHandList = new List<CardSO>();
    protected List<CardSO> cardsInDeckList = new List<CardSO>();
    protected List<CardSO> cardsInDiscardList = new List<CardSO>();
    [Header("OffSet")]
    [SerializeField] float xOffSetMargin = 200; // Define the min/max x margin. Use +ve pls
    [SerializeField] float yOffSetMargin = 40; // Define the min/max y margin. Use +ve pls
    [SerializeField] float zRotationOffSetMargin = 10; // Define the min/max margin. Use +ve pls

    [Header("Prefab Reference")]
    [SerializeField] GameObject cardPrefab;
    protected Transform cardSpawnArea;

    /// <summary>
    /// Get the max HP of the Entity
    /// </summary>
    public int GetMaxHP()
    {
        return maxHP;
    }

    /// <summary>
    /// Get the current HP of the Entity
    /// </summary>
    public int GetCurrentHP()
    {
        return currentHP;
    }

    /// <summary>
    /// Change the value of the current health. Put negative values for minus of health, and vice versa.
    /// Note that player health cannot go above the maximum limit
    /// </summary>
    public virtual void ChangeHealth(int healthChanged)
    {
        currentHP += healthChanged;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    /// <summary>
    /// Change the value of the current health. Put negative values for minus of health, and vice versa.
    /// Note that player health cannot go above the maximum limit
    /// </summary>
    public virtual void ChangeShieldPoint(int shieldPointChanged)
    {
        currentSP += shieldPointChanged;

        if (currentSP > maxSP)
        {
            currentSP = maxSP;
        }
    }

    /// <summary>
    /// Get the max SP of the Entity
    /// </summary>
    public int GetMaxSP()
    {
        return maxSP;
    }

    /// <summary>
    /// Get the current SP of the Entity
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSP()
    {
        return currentSP;
    }

    /// <summary>
    /// Add the card into the deck
    /// </summary>
    public void AddToDeckList(CardSO cardToAdd)
    {
        cardsInDeckList.Add(cardToAdd);
    }

    /// <summary>
    /// Draw cards from the deck into the hand
    /// </summary>
    public virtual void DrawCardFromDeck()
    {
        int cardIndexToGetFrom = Random.Range(0, cardsInDeckList.Count);
        if (cardsInDeckList.Count != 0)
        {
            StartCoroutine(DrawCard(cardsInDeckList[cardIndexToGetFrom]));
            MoveToDifferentList(cardsInDeckList, cardsInHandList, cardsInDeckList[cardIndexToGetFrom]);
        }
        else
            Debug.Log("No cards left to draw!");
    }

    /// <summary>
    /// Play the card, as of whether to do the computing here or not will see first.
    /// But the cards played will be moved to the discardList. If the card has the "Glitch" Keyword,
    /// Do not add it into the discard tile and simply remove it
    /// </summary>
    public void PlayCard(CardSO cardPlayed)
    {
        for (int i = 0; i < cardPlayed.keywordsList.Count; i++)
        {
            if (cardPlayed.keywordsList[i].keyword == KeywordType.Glitch)
            {
                RemoveCardObject(cardPlayed);
                return;
            }
        }

        RemoveCardObject(cardPlayed);
        DisplayCardList();
        MoveToDifferentList(cardsInHandList, cardsInDiscardList, cardPlayed);
    }

    /// <summary>
    /// Remove any card gameobject that has the cardSO type
    /// </summary>
    void RemoveCardObject(CardSO theCard)
    {
        for (int i = 0; i < cardList.Count; i++)
        {
            if (cardList[i].GetComponent<Card>()?.GetCardSO() == theCard)
            {
                Destroy(cardList[i]);
                cardList.RemoveAt(i);
                break;
            }

            else if (cardList[i].GetComponent<EnemyCard>()?.GetCardSO() == theCard)
            {
                Destroy(cardList[i]);
                cardList.RemoveAt(i);
                break;
            }
        }
    }

    /// <summary>
    /// Return card from player hand back to the deck
    /// </summary>
    public void ReturnCardBackToDeck(CardSO cardToReturn)
    {
        RemoveCardObject(cardToReturn);
        MoveToDifferentList(cardsInHandList, cardsInDeckList, cardToReturn);
    }

    /// <summary>
    /// Shuffle all cards from discard list as well as cards on hand back to the deck
    /// </summary>
    public void ReshuffleDeck()
    {
        for (int i = cardList.Count - 1; i >= 0; i--)
        {
            RemoveCardObject(cardList[i].GetComponent<Card>().GetCardSO());
        }
        MoveToDifferentList(cardsInHandList, cardsInDeckList);
        MoveToDifferentList(cardsInDiscardList, cardsInDeckList);
    }

    /// <summary>
    /// Move a card from a list to another (Remove the card from originalList and add it to the newList)
    /// </summary>
    protected virtual void MoveToDifferentList(List<CardSO> originalList, List<CardSO> newList, CardSO cardToMove)
    {
        originalList.Remove(cardToMove);
        newList.Add(cardToMove);
    }

    /// <summary>
    /// Move all cards from the list to another (Remove all card from original list and add them to the newList)
    /// </summary>
    protected virtual void MoveToDifferentList(List<CardSO> originalList, List<CardSO> newList)
    {
        while(originalList.Count != 0)
        {
            CardSO tempCardRef = originalList[0];
            originalList.RemoveAt(0);
            newList.Add(tempCardRef);
        }
    }

    /// <summary>
    /// Draw a card and add it into the list
    /// </summary>
    /// <returns></returns>
    IEnumerator DrawCard(CardSO cardDrawn)
    {
        GameObject newCard = Instantiate(cardPrefab, cardSpawnArea);
        newCard.GetComponent<Card>()?.UpdateCardDetails(cardDrawn);
        newCard.GetComponent<EnemyCard>()?.SetCardSO(cardDrawn);
        cardList.Add(newCard);
        DisplayCardList();
        yield return null;
    }

    /// <summary>
    /// Rearrange the cards to make it more beautiful
    /// </summary>
    void DisplayCardList()
    {
        int totalCard = cardList.Count;

        float xDistanceBetweenInterval = xOffSetMargin * 2 / (totalCard + 1);
        float yDistanceBetweenInterval = yOffSetMargin / ((totalCard + 2) / 2);

        float rotationBetweenInterval = zRotationOffSetMargin * 2 / (totalCard + 1);

        float xPos = -xOffSetMargin;
        float yPos = -yOffSetMargin;
        float zRot = zRotationOffSetMargin;

        for (int i = 0; i < totalCard; i++)
        {
            // move to the next interval and then assign the position and rotation accordingly
            xPos += xDistanceBetweenInterval;
            zRot -= rotationBetweenInterval;

            // if the i value is below half of total, increase the yPos value, otherwise decrease it instead
            if (i > totalCard / 2)
            {
                yPos -= yDistanceBetweenInterval;
            }
            else if (i < totalCard / 2)
            {
                yPos += yDistanceBetweenInterval;
            }
            else if (totalCard % 2 != 0)
            {
                yPos = 0;
            }

            cardList[i].transform.localPosition = new Vector3(xPos, yPos, cardList[i].transform.localPosition.z);
            cardList[i].transform.eulerAngles = new Vector3(0, 0, zRot);
        }
    }
}
