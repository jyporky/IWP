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
    /// If set to true, instead of doing the value per duration, the value will be done after set duration instead.
    /// </summary>
    public bool delay;
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
    Shuffle,
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

/// <summary>
/// The entity who casted the attack
/// </summary>
public enum Caster
{
    PLAYER,
    ENEMY,
}

public class StatusEffectInfo
{
    public int duration;
    public int value;
    public bool delay;
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

    public void ExecuteCard(CardSO card, Player player, EnemyBase enemy, Caster castFrom)
    {
        for (int i = 0; i < card.keywordsList.Count; i++)
        {
            int kwValue = card.keywordsList[i].value;
            KeywordType kw = card.keywordsList[i].keyword;
            int duration = card.keywordsList[i].duration;
            bool delay = card.keywordsList[i].delay;
            Caster castTo = GetCastTo(castFrom, card.keywordsList[i].inflictSelf);

            if (kw == KeywordType.Glitch)
                continue;

            // duration being 0 means it is instant, and no status effect is applied
            if (duration == 0)
            {
                switch (castTo)
                {
                    case Caster.PLAYER:
                        {
                            switch (kw)
                            {
                                case KeywordType.Damage:
                                    player.ChangeHealth(-kwValue);
                                    break;
                                case KeywordType.Draw_Card:
                                    player.DrawCardFromDeck(kwValue);
                                    break;
                                case KeywordType.Heal:
                                    player.ChangeHealth(kwValue);
                                    break;
                                case KeywordType.Gain_SP:
                                    player.ChangeShieldPoint(kwValue);
                                    break;
                            }
                        }
                        break;

                    case Caster.ENEMY:
                        {
                            switch (kw)
                            {
                                case KeywordType.Damage:
                                    enemy.ChangeHealth(-kwValue);
                                    break;
                                case KeywordType.Draw_Card:
                                    enemy.DrawCardFromDeck(kwValue);
                                    break;
                                case KeywordType.Heal:
                                    enemy.ChangeHealth(kwValue);
                                    break;
                                case KeywordType.Gain_SP:
                                    enemy.ChangeShieldPoint(kwValue);
                                    break;
                            }
                        }
                        break;
                }
            }

            else
            {
                StatusEffectInfo newStatusEffectInfo = new StatusEffectInfo();
                newStatusEffectInfo.value = kwValue;
                newStatusEffectInfo.duration = duration;
                newStatusEffectInfo.delay = delay;

                switch (castTo)
                {
                    case Caster.PLAYER:
                        player.AddStatusEffect(kw, newStatusEffectInfo);
                        break;
                    case Caster.ENEMY:
                        enemy.AddStatusEffect(kw, newStatusEffectInfo);
                        break;
                }
            }
        }
    }

    Caster GetCastTo(Caster castToWho, bool targetSelf)
    {
        switch (castToWho)
        {
            case Caster.PLAYER:
                {
                    if (!targetSelf)
                        return Caster.ENEMY;

                    break;
                }

            case Caster.ENEMY:
                {
                    if (!targetSelf)
                        return Caster.PLAYER;

                    break;
                }
        }

        return castToWho;
    }
}
