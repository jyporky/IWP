using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

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

    [Header("DeckAndDiscard")]
    [SerializeField] int startTurnDrawAmt;
    [SerializeField] TextMeshProUGUI deckAmt;
    [SerializeField] TextMeshProUGUI discardAmt;

    [Header("StatusEffect")]
    [SerializeField] GameObject statusPrefab;
    [SerializeField] Transform statusHolder;

    [Header("Entity Information")]
    [SerializeField] protected Image entitySprite;
    [SerializeField] protected TextMeshProUGUI entityNameDisplay;

    public delegate void OnEntityStartTurn();
    /// <summary>
    /// This is called when the entity starts their turn. Reduce the turn value by 1.
    /// </summary>
    public OnEntityStartTurn onEntityStartTurn;

    public delegate void OnEntityEndTurn();
    /// <summary>
    /// This is called when the entity ends their turn. Could use this to delete any card played status effect.
    /// </summary>
    public OnEntityStartTurn onEntityEndTurn;

    /// <summary>
    /// This is called when the entity plays a card. Reduce the card played effect by 1.
    /// </summary>
    public delegate void OnEntityPlayCard();
    public OnEntityPlayCard onEntityPlayCard;

    /// <summary>
    /// Store the list of status effect the entity has. Postive value suggest turns, negative value suggest by cards played.
    /// If the value reaches 0, that effect no longer exist and will be removed.
    /// </summary>
    protected Dictionary<KeywordType, List<GameObject>> statusEffectList = new Dictionary<KeywordType, List<GameObject>>();

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
    /// Note that player health cannot go above the maximum limit. Negative health change can be increased with Marked debuff.
    /// </summary>
    public virtual void ChangeHealth(int healthChanged)
    {
        currentHP += healthChanged;
        if (statusEffectList.ContainsKey(KeywordType.Marked) && healthChanged < 0)
        {
            for (int i = 0; i < statusEffectList[KeywordType.Marked].Count; i++)
            {
                currentHP -= statusEffectList[KeywordType.Marked][i].GetComponent<StatusEffect>().GetValue();
            }
        }

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
    /// Trigger the start turn effect of the entity, such as drawing cards and triggering any start turn effect.
    /// </summary>
    public void StartTurn()
    {
        TriggerEffect(false);
        DrawCardFromDeck(startTurnDrawAmt);
        onEntityStartTurn?.Invoke();
    }

    /// <summary>
    /// Trigger the end turn effect of the entity, such as drawing cards and triggering any start turn effect.
    /// </summary>
    public void EndTurn()
    {
        CheckIfNeedReshuffle();
        onEntityEndTurn?.Invoke();
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
    void DrawCardFromDeck()
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
    /// Draw cards from the deck into the hand. Specify the number needed
    /// </summary>
    public void DrawCardFromDeck(int amtOfCardsToDraw)
    {
        for (int i = 0; i < amtOfCardsToDraw; i++)
        {
            DrawCardFromDeck();
        }
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
            if (cardPlayed.keywordsList[i].keywordType == KeywordType.Glitch)
            {
                RemoveCardObject(cardPlayed);
                return;
            }
        }

        TriggerEffect(true);
        onEntityPlayCard?.Invoke();
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
            if (cardList[i].GetComponent<CardBase>()?.GetCardSO() == theCard)
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
            RemoveCardObject(cardList[i].GetComponent<CardBase>().GetCardSO());
        }
        MoveToDifferentList(cardsInHandList, cardsInDeckList);
        MoveToDifferentList(cardsInDiscardList, cardsInDeckList);
    }

    /// <summary>
    /// Move a card from a list to another (Remove the card from originalList and add it to the newList)
    /// </summary>
    void MoveToDifferentList(List<CardSO> originalList, List<CardSO> newList, CardSO cardToMove)
    {
        originalList.Remove(cardToMove);
        newList.Add(cardToMove);
        UpdateDeckAndDiscardAmountDisplay();
    }

    /// <summary>
    /// Move all cards from the list to another (Remove all card from original list and add them to the newList)
    /// </summary>
    void MoveToDifferentList(List<CardSO> originalList, List<CardSO> newList)
    {
        while (originalList.Count != 0)
        {
            CardSO tempCardRef = originalList[0];
            originalList.RemoveAt(0);
            newList.Add(tempCardRef);
            UpdateDeckAndDiscardAmountDisplay();
        }
    }

    /// <summary>
    /// Draw a card and add it into the list
    /// </summary>
    /// <returns></returns>
    IEnumerator DrawCard(CardSO cardDrawn)
    {
        GameObject newCard = Instantiate(cardPrefab, cardSpawnArea);
        newCard.GetComponent<CardBase>()?.UpdateCardDetails(cardDrawn);
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

    /// <summary>
    /// Check to see if the entity deck needs to be reshuffled. Deck will be automatically
    /// reshuffle if entity has no cards left in their deck
    /// </summary>
    void CheckIfNeedReshuffle()
    {
        if (cardsInDeckList.Count == 0)
        {
            ReshuffleDeck();
        }
    }

    /// <summary>
    /// Update the display for the amount of cards in the deck and the discard pile.
    /// </summary>
    protected void UpdateDeckAndDiscardAmountDisplay()
    {
        deckAmt.text = cardsInDeckList.Count.ToString();
        discardAmt.text = cardsInDiscardList.Count.ToString();
    }

    /// <summary>
    /// Add a status effect to the entity. when stating duration value, positive mean turn, negative means by amt of cards. Note that amt of cards
    /// will reset when player end their turn.
    /// </summary>
    public void AddStatusEffect(Keyword statusEffectInfo)
    {
        Keyword newKeyword = new Keyword(statusEffectInfo);
        // check to see if the status effect with that gameobject exist. If it exist and has the same duration, stack the value.
        if (statusEffectList.ContainsKey(newKeyword.keywordType) && GetExistingStatusEffect(newKeyword) != null)
        {
            GameObject statusReference = GetExistingStatusEffect(newKeyword);
            statusReference.GetComponent<StatusEffect>().UpdateValue(newKeyword.value);
        }

        else
        {
            GameObject newStatus = Instantiate(statusPrefab, statusHolder);
            newStatus.GetComponent<StatusEffect>().SetStatus(this, newKeyword);

            // if there is already a list in that key, simply add the new status to that list
            if (statusEffectList.ContainsKey(newKeyword.keywordType))
            {
                statusEffectList[newKeyword.keywordType].Add(newStatus);
            }
            // if there is no list in that key, create the new status, then create a dictionary list and add it to the statuseffectList.
            else
            {
                List<GameObject> newStatusList = new List<GameObject>();
                newStatusList.Add(newStatus);
                statusEffectList.Add(newKeyword.keywordType, newStatusList);
            }
        }
    }

    /// <summary>
    /// Get any existing gameobject with the status effect IF the duration matches as well as whether they are delayed.
    /// </summary>
    /// <returns></returns>
    GameObject GetExistingStatusEffect(Keyword statusEffect)
    {
        List<GameObject> statusList = statusEffectList[statusEffect.keywordType];
        for (int i = 0; i < statusList.Count; i++)
        {
            StatusEffect se = statusList[i].GetComponent<StatusEffect>();
            if ((statusEffect.durationByTurn == se.IsDurationByTurn()) && (statusEffect.cardDelay.statusInfo != null == se.IsDelay())
                && (statusEffect.duration == se.GetDuration() || statusEffect.cardDelay.duration == se.GetDuration()))
            {
                return statusList[i];
            }
        }

        return null;
    }

    /// <summary>
    /// When a status effect expire, call this function to remove the object from the keyword status list. If that keyword status list is empty. Delete
    /// that keyword from the dictionary.
    /// </summary>
    public void RemoveStatusEffect(KeywordType removeWhatStatus, GameObject objectReference)
    {
        statusEffectList[removeWhatStatus].Remove(objectReference);
        if (statusEffectList[removeWhatStatus].Count == 0)
        {
            statusEffectList.Remove(removeWhatStatus);
        }
    }

    /// <summary>
    /// Calculate the damage value of the attack, along with any buff that affects it. The buff include: Strength and Weaken.
    /// </summary>
    public int CalculateDamage(int attackdmg)
    {
        int finalDmg = attackdmg;
        // calculate the attack buff modifier, if it is not a delay
        if (statusEffectList.ContainsKey(KeywordType.Strength))
        {
            for (int i = 0; i < statusEffectList[KeywordType.Strength].Count; i++)
            {
                finalDmg += statusEffectList[KeywordType.Strength][i].GetComponent<StatusEffect>().GetValue();
            }
        }

        // caculate the attack debuff modifier, if it is not a delay
        if (statusEffectList.ContainsKey(KeywordType.Weaken))
        {
            for (int i = 0; i < statusEffectList[KeywordType.Weaken].Count; i++)
            {
                finalDmg += statusEffectList[KeywordType.Weaken][i].GetComponent<StatusEffect>().GetValue();
            }
        }

        // if dmg is negative, set it to 0
        if (finalDmg < 0)
            finalDmg = 0;

        return finalDmg;
    }

    /// <summary>
    /// Trigger the effect of damage, gain_Sp, heal and draw
    /// </summary>
    void TriggerEffect(bool byCardPlayed)
    {
        if (statusEffectList.ContainsKey(KeywordType.Damage))
        {
            for (int i = 0; i < statusEffectList[KeywordType.Damage].Count; i++)
            {
                StatusEffect statusEffect = statusEffectList[KeywordType.Damage][i].GetComponent<StatusEffect>();
                if (statusEffect.IsDelay())
                    continue;
                if (statusEffect.IsDurationByTurn() && !byCardPlayed || !statusEffect.IsDurationByTurn() && byCardPlayed)
                {
                    ChangeHealth(-statusEffect.GetValue());
                }
            }
        }

        if (statusEffectList.ContainsKey(KeywordType.Heal))
        {
            for (int i = 0; i < statusEffectList[KeywordType.Heal].Count; i++)
            {
                StatusEffect statusEffect = statusEffectList[KeywordType.Heal][i].GetComponent<StatusEffect>();
                if (statusEffect.IsDelay())
                    continue;
                if (statusEffect.IsDurationByTurn() && !byCardPlayed || !statusEffect.IsDurationByTurn() && byCardPlayed)
                {
                    ChangeHealth(statusEffect.GetValue());
                }
            }
        }

        if (statusEffectList.ContainsKey(KeywordType.Draw_Card))
        {
            for (int i = 0; i < statusEffectList[KeywordType.Draw_Card].Count; i++)
            {
                StatusEffect statusEffect = statusEffectList[KeywordType.Draw_Card][i].GetComponent<StatusEffect>();
                if (statusEffect.IsDelay())
                    continue;
                if (statusEffect.IsDurationByTurn() && !byCardPlayed || !statusEffect.IsDurationByTurn() && byCardPlayed)
                {
                    DrawCardFromDeck(statusEffect.GetValue());
                }
            }
        }

        if (statusEffectList.ContainsKey(KeywordType.Gain_Energy_Point))
        {
            for (int i = 0; i < statusEffectList[KeywordType.Gain_Energy_Point].Count; i++)
            {
                StatusEffect statusEffect = statusEffectList[KeywordType.Gain_Energy_Point][i].GetComponent<StatusEffect>();
                if (statusEffect.IsDelay())
                    continue;
                if (statusEffect.IsDurationByTurn() && !byCardPlayed || !statusEffect.IsDurationByTurn() && byCardPlayed)
                {
                    ChangeShieldPoint(statusEffect.GetValue());
                }
            }
        }
    }

    /// <summary>
    /// Reshuffle cards. Remove all status effect.
    /// </summary>
    public void RefreshStatusAndDeck()
    {
        foreach (KeyValuePair<KeywordType, List<GameObject>> kw in statusEffectList)
        {
            for (int i = 0; i < kw.Value.Count; i = 0)
            {
                Destroy(kw.Value[0]);
            }
        }

        statusEffectList.Clear();
        ReshuffleDeck();
    }
}
