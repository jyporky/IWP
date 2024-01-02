using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    private static AssetManager instance;
    public static AssetManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


    [System.Serializable]
    private class CardTypeSprite
    {
        public CardType cardType;
        public Sprite cardSprite;
    }

    [System.Serializable]
    private class CardTypeShieldReference
    {
        public CardType cardType;
        public CardSO cardShieldSO;
    }

    [SerializeField] CardTypeSprite[] cardTypeSpriteList;
    [SerializeField] CardTypeShieldReference[] cardTypeShieldList;
    [SerializeField] GameObject interactableCardPrefab;
    [SerializeField] GameObject enemyCardPrefab;
    [SerializeField] GameObject selectedCardPrefab;
    [SerializeField] GameObject perkPrefab;
    [SerializeField] GameObject statusPrefab;
    [SerializeField] Keyword overloadedEffect;


    /// <summary>
    /// Get the cardType sprite according to the cardType.
    /// </summary>
    public Sprite GetCardSprite(CardType cardType)
    {
        foreach(var card in cardTypeSpriteList)
        {
            if (cardType == card.cardType)
            {
                return card.cardSprite;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the cardSO reference according to the cardType.
    /// </summary>
    public CardSO GetCardShieldSO(CardType cardType)
    {
        foreach(var card in cardTypeShieldList)
        {
            if (cardType == card.cardType)
            {
                return card.cardShieldSO;
            }
        }

        return null;
    }

    /// <summary>
    /// Get the perk prefab.
    /// </summary>
    public GameObject GetPerkPrefab()
    {
        return perkPrefab;
    }

    /// <summary>
    /// Get the status prefab.
    /// </summary>
    public GameObject GetStatusPrefab()
    {
        return statusPrefab;
    }

    /// <summary>
    /// Get the overloaded Keyword, able to further modify the duration of the overload in turns.
    /// </summary>
    public Keyword GetOverloadedEffect(int duration)
    {
        Keyword returnKeyword = new Keyword(overloadedEffect);

        returnKeyword.duration = duration;

        return returnKeyword;
    }
}
