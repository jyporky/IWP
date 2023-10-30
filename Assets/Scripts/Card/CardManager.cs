using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class Keyword
{
    /// <summary>
    /// The keywordSO reference which stores the keyword, byTurn boolean as well as delay boolean.
    /// </summary>
    public KeywordSO keywordSO;

    /// <summary>
    /// The value of the keyword, put negative for multipler instead
    /// </summary>
    public int value;

    /// <summary>
    /// State how long the effect stays, 0 is instant.
    /// </summary>
    public int duration;

    /// <summary>
    /// Inflict this effect to yourself. Default to false
    /// </summary>
    public bool inflictSelf;

    /// <summary>
    /// Define the delayDuration of the delay, positive number to indicate turns and negative number to indicate cards played.
    /// Noted that if cards played duration is not fulfilled when turns end, the effect will not be applied.
    /// </summary>
    public int delayDuration;
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
            if (card.keywordsList[i].keywordSO.keyword == KeywordType.Glitch)
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
        KeywordType kw = keyword.keywordSO.keyword;
        int duration = keyword.duration;
        bool inflictSelf = keyword.inflictSelf;
        bool delay = keyword.keywordSO.delay;

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