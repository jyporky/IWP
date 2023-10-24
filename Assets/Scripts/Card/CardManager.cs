using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[System.Serializable]
public class Keyword
{
    public KeywordType keyword;
    public int value;
}

public enum KeywordType
{
    NONE,
    Damage,
    Heal,
    Draw_Card,
    Glitch,
}

public enum CardType
{
    Offensive,
    Utility,
}

public enum AttackCardType
{
    NONE,
    Light,
    Normal,
    Heavy,
}

public enum CastToWho
{
    PLAYERTOENEMY,
    ENEMYTOPLAYER,
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

    public void ExecuteCard(CardSO card, Player player, EnemyBase enemy, CastToWho castToWho)
    {
        for (int i = 0; i < card.keywordsList.Count; i++)
        {
            int kwValue = card.keywordsList[i].value;
            KeywordType kw = card.keywordsList[i].keyword;

            // Draw amt of card according to value
            if (kw == KeywordType.Draw_Card)
            {
                for (int value = 0; value < kwValue; value++)
                {
                    switch (castToWho)
                    {
                        case CastToWho.PLAYERTOENEMY:
                            player.DrawCardFromDeck();
                            break;
                        case CastToWho.ENEMYTOPLAYER:
                            enemy.DrawCardFromDeck();
                            break;
                    }
                }
            }

            // Do damage to target according to value
            else if (kw == KeywordType.Damage)
            {
                switch (castToWho)
                {
                    case CastToWho.PLAYERTOENEMY:
                        enemy.ChangeHealth(-kwValue);
                        break;
                    case CastToWho.ENEMYTOPLAYER:
                        player.ChangeHealth(-kwValue);
                        break;
                }
            }

            else if (kw == KeywordType.Heal)
            {
                switch (castToWho)
                {
                    case CastToWho.PLAYERTOENEMY:
                        player.ChangeHealth(kwValue);
                        break;
                    case CastToWho.ENEMYTOPLAYER:
                        enemy.ChangeHealth(kwValue);
                        break;
                }
            }
        }
    }
}
