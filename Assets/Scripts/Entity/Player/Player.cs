using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : Entity
{
    [Header("UI references")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] Slider shieldPointBarSlider;
    [SerializeField] TextMeshProUGUI shieldPointValue;
    [SerializeField] Button lightShieldButton;
    [SerializeField] Button normalShieldButton;
    [SerializeField] Button heavyShieldButton;

    [Header("Deck List Reference")]
    [SerializeField] GameObject deckListUIPrefab;
    [SerializeField] Button deckListButton;
    [SerializeField] Button discardListButton;
    private Transform UISpawnArea;

    private void Awake()
    {
        // Link the cardspawnarea
        cardSpawnArea = GameObject.FindGameObjectWithTag("PlayerCardSpawn").transform;
    }

    private void Start()
    {
        UISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
        deckListButton.onClick.AddListener(OpenDeckUIPanel);
        discardListButton.onClick.AddListener(OpenDiscardUIPanel);
    }

    private void OnEnable()
    {
        LoadStats();
        LoadCardToDeck();
        ChangeHealth(0);
        ChangeShieldPoint(0);
        UpdateDeckAndDiscardAmountDisplay();
    }

    /// <summary>
    /// Load the deck from the playerInfo into the gameplay Player
    /// </summary>
    void LoadCardToDeck()
    {
        List<CardSO> cardList = PlayerManager.GetInstance().GetCardList();
        for (int i = 0; i < cardList.Count; i++)
        {
            cardsInDeckList.Add(cardList[i]);
        }
    }

    /// <summary>
    /// Load the player stats to the Entity details
    /// </summary>
    void LoadStats()
    {
        PlayerManager pm = PlayerManager.GetInstance();
        maxHP = pm.GetMaxHP();
        currentHP = pm.GetCurrentHealth();
        maxSP = pm.GetMaxEP();
        currentSP = pm.GetCurrentEP();
    }
    
    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);
        healthBarSlider.value = (float)currentHP / maxHP;
        healthValue.text = currentHP.ToString() + "/" + maxHP.ToString();
        if (currentHP <= 0)
        {
            CombatManager.GetInstance().SetGameOver(true);
        }
    }

    public override void ChangeShieldPoint(int shieldPointChanged)
    {
        base.ChangeShieldPoint(shieldPointChanged);
        shieldPointBarSlider.value = (float)currentSP / maxSP;
        shieldPointValue.text = currentSP.ToString() + "/" + maxSP.ToString();
    }

    /// <summary>
    /// Create a deck list panel, put in the cards left in their deck
    /// </summary>
    void OpenDeckUIPanel()
    {
        GameObject newDeckListPanel = Instantiate(deckListUIPrefab, UISpawnArea);
        newDeckListPanel.GetComponent<DeckList>().SetupDeckList(cardsInDeckList);
    }

    /// <summary>
    /// Create a deck list panel, put in the cards left in their deck
    /// </summary>
    void OpenDiscardUIPanel()
    {
        GameObject newDiscardListPanel = Instantiate(deckListUIPrefab, UISpawnArea);
        newDiscardListPanel.GetComponent<DeckList>().SetupDeckList(cardsInDiscardList, "Discard List");
    }
}
