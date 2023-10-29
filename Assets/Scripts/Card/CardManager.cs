using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class Keyword
{
    public KeywordType keyword;
    /// <summary>
    /// The value of the keyword, put negative for multipler instead
    /// </summary>
    public int value;
    /// <summary>
    /// State how long the effect stays, 0 is instant.
    /// Positive indicate turns, Negative indicate by how many card played.
    /// </summary>
    public int duration;
    /// <summary>
    /// Inflict this effect to yourself. Default to false
    /// </summary>
    public bool inflictSelf;

    /// <summary>
    /// If set to true, wait for the delayDuration to be over before executing the effect.
    /// </summary>
    public bool delay;

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

public class StatusEffectInfo
{
    public int duration;
    public int value;

    public bool delay;
    public int delayDuration;
    public Entity targetting;
}

public class CardManager : MonoBehaviour
{
    [SerializeField] List<StatusSpriteInfo> statusSpriteInfoList = new List<StatusSpriteInfo>();

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
    /// Execute the card. The caster is the one who is attacking, and the target is the one receivng the attack
    /// </summary>
    public void ExecuteCard(CardSO card, Entity caster, Entity target)
    {
        for (int i = 0; i < card.keywordsList.Count; i++)
        {
            if (card.keywordsList[i].keyword == KeywordType.Glitch)
                continue;

            ExecuteKeywordEffect(card.keywordsList[i], caster, target);
        }
    }

    /// <summary>
    /// Execute the card 
    /// </summary>
    public void ExecuteCardFromDelay(Entity caster, StatusEffectInfo statusInfo, KeywordType whichStatus)
    {
        Keyword kw = new Keyword();
        kw.keyword = whichStatus;
        kw.value = statusInfo.value;
        kw.duration = statusInfo.duration;
        kw.inflictSelf = true;
        kw.delay = false;
        kw.delayDuration = 0;
        ExecuteKeywordEffect(kw, caster, statusInfo.targetting);
    }

    /// <summary>
    /// Execute the keywordType effect given the keyword class.
    /// </summary>
    public void ExecuteKeywordEffect(Keyword keyword, Entity caster, Entity target)
    {
        int kwValue = keyword.value;
        KeywordType kw = keyword.keyword;
        int duration = keyword.duration;
        bool inflictSelf = keyword.inflictSelf;
        bool delay = keyword.delay;
        int delayduration = keyword.delayDuration;

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
            StatusEffectInfo newStatusEffectInfo = new StatusEffectInfo();
            newStatusEffectInfo.value = kwValue;
            newStatusEffectInfo.duration = duration;
            newStatusEffectInfo.delay = delay;
            newStatusEffectInfo.delayDuration = delayduration;
            newStatusEffectInfo.targetting = targetEntity;

            // if there is no delay. Simply add those debuff to the target
            if (!delay)
            {
                targetEntity.AddStatusEffect(kw, newStatusEffectInfo);
            }
            // if there is a delay, show it to themselves
            else
            {
                caster.AddStatusEffect(kw, newStatusEffectInfo);
            }
        }
    }

    /// <summary>
    /// Get the sprite of the status effect. Requires the statusEffectInfo as well as who is the one having the status.
    /// </summary>
    public StatusSpriteInfo GetStatusSpriteInfo(StatusEffectInfo statusEffect, KeywordType statusType)
    {
        for (int i = 0; i < statusSpriteInfoList.Count; i++)
        {
            if (statusSpriteInfoList[i].whichStatus == statusType &&
                statusSpriteInfoList[i].isDelay == statusEffect.delay &&
                statusSpriteInfoList[i].byTurn == (statusEffect.duration > 0))
            {
                return statusSpriteInfoList[i];
            }
        }

        return null;
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

[System.Serializable]
public class StatusSpriteInfo
{
    public string statusName;
    [TextArea]
    public string statusDescription;
    public Sprite statusSprite;
    [Header("Card Info")]
    public KeywordType whichStatus;
    public bool isDelay;
    public bool byTurn;
}