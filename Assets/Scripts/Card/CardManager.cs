using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
    /// Copy all the values except the cardDelay from the keywordToCopy to the new Keyword reference.
    /// </summary>
    public Keyword(Keyword keywordToCopy)
    {
        keywordType = keywordToCopy.keywordType;
        value = keywordToCopy.value;
        inflictSelf = keywordToCopy.inflictSelf;
        duration = keywordToCopy.duration;
        durationByTurn = keywordToCopy.durationByTurn;
        statusSO = keywordToCopy.statusSO;
        cardDelay = new CardDelay();
    }
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
    Gain_SP,
    Marked,
    Strength,
    Weaken,
    Glitch,
    Trojan,
    Armor,
}

public enum CardType
{
    Offensive,
    Defensive,
    Utility,
}

public enum AttackCardType
{
    NONE,
    Light,
    Normal,
    Heavy,
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

    /// <summary>
    /// this will be set to true if the forcestop function is called. It will stop any ongoing card effect if there is any.
    /// </summary>
    private bool forceStop = false;

    /// <summary>
    /// Execute the card. The caster is the one who is attacking, and the target is the one receivng the attack
    /// </summary>
    public void ExecuteCard(CardSO card, Entity caster, Entity target)
    {
        for (int i = 0; i < card.keywordsList.Count; i++)
        {
            if (card.keywordsList[i].keywordType == KeywordType.Glitch)
                continue;

            if (forceStop)
                break;

            ExecuteKeywordEffect(card.keywordsList[i], caster, target);
        }
        forceStop = false;
    }

    /// <summary>
    /// Execute the keywordType effect given the keyword class.
    /// </summary>
    public void ExecuteKeywordEffect(Keyword keyword, Entity caster, Entity target)
    {
        int kwValue = keyword.value;
        KeywordType kw = keyword.keywordType;
        int duration = keyword.duration;
        bool inflictSelf = keyword.inflictSelf;
        bool delay = (keyword.cardDelay.statusInfo != null);

        Entity targetEntity = GetTarget(caster, target, inflictSelf);

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
                case KeywordType.Gain_SP:
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
    public Entity GetTarget(Entity caster, Entity target, bool inflictSelf)
    {
        if (inflictSelf)
            return caster;
        else
            return target;
    }
}