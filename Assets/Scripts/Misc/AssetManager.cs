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
    /// Get the card prefab, this depends on whether it is player or enemy. <br/>
    /// Return player card if bool is false. <br/>
    /// Return enemy card if bool is true. <br/>
    /// </summary>
    public GameObject GetEnemyCardPrefab(bool isEnemy)
    {
        switch (isEnemy)
        {
            case true:
                return enemyCardPrefab;
            case false:
                return interactableCardPrefab;
        }
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
    /// Get the Selected Card Prefab, the one that displays the keywords of a card.
    /// </summary>
    public GameObject GetSelectedCardPrefab()
    {
        return selectedCardPrefab;
    }
}
