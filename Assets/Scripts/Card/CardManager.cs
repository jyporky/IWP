using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class Keyword
{
    /// <summary>
    /// The keywordType of the keyword.
    /// </summary>
    public KeywordType keywordType;

    /// <summary>
    /// The value of the keyword, put negative for multipler instead
    /// </summary>
    public int value;

    /// <summary>
    /// Inflict this effect to yourself. Default to false
    /// </summary>
    public bool inflictSelf;

    /// <summary>
    /// State how long the effect stays, 0 is instant.
    /// </summary>
    public int duration;

    /// <summary>
    /// If true, the duration is based on turn. It will be reduced when the entity turn starts.
    /// If false, the duration is based on card played. It will be reduced when the entity plays a card.
    /// Card played duration will be removed at the end of the turn automatically.
    /// </summary>
    public bool durationByTurn;

    /// <summary>
    /// Stores the status sprite, name and description information. leave this empty if the effect is instant.
    /// </summary>
    public StatusSO statusSO;

    /// <summary>
    /// If there is a delay before playing the card, give the status information, the duration of the delay and whether it is by cards played or turns.
    /// This will be the status effect instead of the keyword class. Once the delay duration reaches 0, it will be removed and trigger
    /// the keyword class.
    /// </summary>
    public CardDelay cardDelay;

    /// <summary>
    /// The card effect can be further modify by amount of cards if needed. Set where to count the amount of cards as well as what specific type of card to check. <br/>
    /// If no specific type of card is stated, put NONE. If there is no need to modify, put NONE on the modifyByAmountOfCardType.
    /// </summary>
    public ModifyByAmountOfCards modifyEffectByAmountOfCards;

    /// <summary>
    /// Copy all the values except the cardDelay from the keywordToCopy to the new Keyword reference.
    /// If ignoreCardDelay is true, create a new CardDelay otherwise use the copied one.
    /// </summary>
    public Keyword(Keyword keywordToCopy, bool ignoreCardDelay = false)
    {
        keywordType = keywordToCopy.keywordType;
        value = keywordToCopy.value;
        inflictSelf = keywordToCopy.inflictSelf;
        duration = keywordToCopy.duration;
        durationByTurn = keywordToCopy.durationByTurn;
        statusSO = keywordToCopy.statusSO;
        modifyEffectByAmountOfCards = keywordToCopy.modifyEffectByAmountOfCards;
        if (ignoreCardDelay)
            cardDelay = new CardDelay();
        else
            cardDelay = keywordToCopy.cardDelay;
    }
}

[System.Serializable]
public class ModifyByAmountOfCards
{
    public ModifyByAmountOfCardsType modifyByAmountOfCardType;
    public CardType bywhatCardType;
    public int value;
    public CardSO cardReferenceIfAny;
}

[System.Serializable]
public class CardDelay
{
    /// <summary>
    /// The status information to display.
    /// </summary>
    public StatusSO statusInfo;

    /// <summary>
    /// The duration of the delay, cannot be 0.
    /// </summary>
    public int duration;

    /// <summary>
    /// If true, the duration is based on turn. It will be reduced when the entity turn starts.
    /// If false, the duration is based on card played. It will be reduced when the entity plays a card.
    /// If there is an existing card played delay duration, remove the keyword class.
    /// </summary>
    public bool durationByTurn;
}

public enum KeywordType
{
    NONE,
    Damage,
    Draw_Card,
    Heal,
    Gain_Energy_Point,
    Marked,
    Strength,
    Weaken,
    Glitch,
    Trojan,
    Block_Virus,
    Block_Worm,
    Block_Trojan,
}

public enum CardType
{
    None,
    Virus,
    Worm,
    Trojan,
}

public enum ModifyByAmountOfCardsType
{
    NONE,
    BY_CARDS_IN_DRAW_PILE,
    BY_CARDS_IN_HANDS,
    BY_CARDS_IN_DISCARDS,
    BY_DECK,
}

public class CardManager : MonoBehaviour
{
    public static CardManager GetInstance()
    {
        return instance;
    }
    private static CardManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private Entity playerEntity;
    private Entity enemyEntity;

    /// <summary>
    /// Set the player and enemy reference. MAKE SURE TO CALL THIS WHEN COMBAT STARTS.
    /// </summary>
    public void SetPlayerEnemyReference(Entity playerRef, Entity enemyRef)
    {
        playerEntity = playerRef;
        enemyEntity = enemyRef;
    }

    /// <summary>
    /// this will be set to true if the forcestop function is called. It will stop any ongoing card effect if there is any.
    /// </summary>
    private bool forceStop = false;

    /// <summary>
    /// Execute the card. The caster is the one who is attacking, and the target is the one receivng the attack
    /// </summary>
    public void ExecuteCard(CardSO card, Entity caster)
    {
        if (CheckIfCardNegated(caster, card))
            return;

        for (int i = 0; i < card.keywordsList.Count; i++)
        {
            if (card.keywordsList[i].keywordType == KeywordType.Glitch)
                continue;

            if (forceStop)
                break;

            ExecuteKeywordEffect(card.keywordsList[i], caster);
        }
        forceStop = false;
    }

    /// <summary>
    /// Execute the keywordType effect given the keyword class.
    /// </summary>
    public void ExecuteKeywordEffect(Keyword keyword, Entity caster)
    {
        int kwValue = GetEffectValue(keyword, caster);
        KeywordType kw = keyword.keywordType;
        int duration = keyword.duration;
        bool inflictSelf = keyword.inflictSelf;
        bool delay = (keyword.cardDelay.statusInfo != null);

        Entity targetEntity = GetTarget(caster, inflictSelf);

        // duration being 0 means it is instant, and no status effect is applied. However there must not be a delay
        if (duration == 0 && !delay)
        {
            switch (kw)
            {
                case KeywordType.Damage:
                    targetEntity.ChangeHealth(-caster.GetComponent<Entity>().CalculateDamage(kwValue));
                    break;
                case KeywordType.Draw_Card:
                    targetEntity.DrawCardFromDeck(kwValue);
                    break;
                case KeywordType.Heal:
                    targetEntity.ChangeHealth(kwValue);
                    break;
                case KeywordType.Gain_Energy_Point:
                    targetEntity.ChangeShieldPoint(kwValue);
                    break;
            }
        }

        else
        {
            // if there is no delay. Simply add those debuff to the target
            if (!delay)
            {
                targetEntity.AddStatusEffect(keyword);
            }
            // if there is a delay, show it to themselves
            else
            {
                caster.AddStatusEffect(keyword);
            }
        }
    }

    /// <summary>
    /// Force stop any card effect that is still taking place
    /// </summary>
    public void ForceStopCardEffect()
    {
        forceStop = true;
    }

    /// <summary>
    /// Get which entity is the one receving the card effect
    /// </summary>
    public Entity GetTarget(Entity caster, bool inflictSelf)
    {
        switch (caster)
        {
            case Player p:
                if (inflictSelf)
                    return playerEntity;
                else
                    return enemyEntity;

            case EnemyBase e:
                if (inflictSelf)
                    return enemyEntity;
                else
                    return playerEntity;
        }

        return null;
    }

    /// <summary>
    /// Check to see if the card can be negated.
    /// </summary>
    bool CheckIfCardNegated(Entity caster, CardSO cardReference)
    {
        Entity oppositionEntity = GetTarget(caster, false);
        if (oppositionEntity.BlockCardEffect(cardReference.cardType))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Get the card effect value after the modifier if any.
    /// </summary>
    /// <returns></returns>
    int GetEffectValue(Keyword cardEffect, Entity caster)
    {
        if (cardEffect.modifyEffectByAmountOfCards.modifyByAmountOfCardType == ModifyByAmountOfCardsType.NONE)
            return cardEffect.value;

        // Get the list of indicated place.
        int cardValue = cardEffect.value;
        int cardEffectValue = cardEffect.modifyEffectByAmountOfCards.value;
        List<CardSO> cardList = new List<CardSO>();
        cardList = caster.GetCardList(cardEffect.modifyEffectByAmountOfCards.modifyByAmountOfCardType);     

        int counter = 0;
        // Check to see if there is any cardReference, if there is a card reference, add counter if that matches. Otherwise add counter if card type matches.
        CardSO cardReference = cardEffect.modifyEffectByAmountOfCards.cardReferenceIfAny;
        if (cardReference == null)
        {
            // Get the card type
            CardType whatCardType = cardEffect.modifyEffectByAmountOfCards.bywhatCardType;

            // If there is no specific card type, set the counter to the list count.
            if (whatCardType == CardType.None)
                counter = cardList.Count;

            // if the cardtype matches, add one to the counter.
            else
            {
                foreach (CardSO card in cardList)
                {
                    if (card.cardType == whatCardType)
                    {
                        counter++;
                    }
                }
            }
        }
        else
        {
            foreach(CardSO card in cardList)
            {
                if (card == cardReference)
                {
                    counter++;
                }
            }
        }

        int returnValue = cardValue + (cardEffectValue * counter);
        // should the return value invert from negative to positive or the other way around, negate it and set it to 0.
        if (cardValue > 0)
        {
            returnValue = Mathf.Max(0, returnValue);
        }
        else if (cardValue < 0)
        {
            returnValue = Mathf.Min(0, returnValue);
        }

        // return the value according to the counter multiply by the indicated value.
        return returnValue;
    }
}