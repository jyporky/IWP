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
    [SerializeField] Button virusShieldButton;
    [SerializeField] Button wormShieldButton;
    [SerializeField] Button trojanShieldButton;

    [Header("Deck List Reference")]
    [SerializeField] GameObject deckListUIPrefab;
    [SerializeField] Button deckListButton;
    [SerializeField] Button discardListButton;
    private Transform UISpawnArea;

    [Header("Hack Information")]
    [SerializeField] GameObject hackDisplayPrefab;
    [SerializeField] Transform hackListTransform;

    [Header("Hacking Counter")]
    [SerializeField] GameObject cardTypeDisplayPrefab;
    [SerializeField] Transform cardTypeIconListReference;

    private HackingManager hm;
    HackDisplay[] hackDisplayList = null;
    private List<CardType> hackCounterList = new List<CardType>();
    private List<GameObject> iconDisplayList = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
        // Link the cardspawnarea
        cardSpawnArea = GameObject.FindGameObjectWithTag("PlayerCardSpawn").transform;
        hm = HackingManager.GetInstance();
        cardPrefab = am.GetEnemyCardPrefab(false);
    }

    private void Start()
    {
        UISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
        deckListButton.onClick.AddListener(OpenDeckUIPanel);
        discardListButton.onClick.AddListener(OpenDiscardUIPanel);
        virusShieldButton.onClick.AddListener(delegate { DoBlockEffect(CardType.Virus); });
        wormShieldButton.onClick.AddListener(delegate { DoBlockEffect(CardType.Worm); });
        trojanShieldButton.onClick.AddListener(delegate { DoBlockEffect(CardType.Trojan); });
    }

    private void OnEnable()
    {
        LoadStats();
        LoadCardToDeck();
        ChangeHealth(0);
        ChangeShieldPoint(0);
        UpdateDeckAndDiscardAmountDisplay();

        GameObject newHackInfo = Instantiate(hackDisplayPrefab, hackListTransform);
        newHackInfo.GetComponent<HackDisplay>().SetHackType(HackType.More_Nexus_Core, this);

        // Get the list of hacks in the combat
        hackDisplayList = hackListTransform.GetComponentsInChildren<HackDisplay>();
    }

    /// <summary>
    /// Play the selected card, add the card type into the counter.
    /// </summary>
    /// <param name="cardPlayed"></param>
    public override void PlayCard(CardSO cardPlayed)
    {
        base.PlayCard(cardPlayed);
        hackCounterList.Add(cardPlayed.cardType);
        GameObject iconDisplay = Instantiate(cardTypeDisplayPrefab, cardTypeIconListReference.transform);
        iconDisplay.GetComponent<Image>().sprite = am.GetCardSprite(cardPlayed.cardType);
        iconDisplayList.Add(iconDisplay);


        foreach (var r in hackDisplayList)
        {
            r.AttemptHack(hackCounterList);
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();

        foreach(var r in hackDisplayList)
        {
            ExecuteHackEffect(r.GetHackType(), r.GetHackLvl());
            r.ReduceDuration();
        }
    }

    public override void EndTurn()
    {
        base.EndTurn();
        hackCounterList.Clear();
        for (int i = iconDisplayList.Count - 1; i >= 0; i--)
        {
            Destroy(iconDisplayList[i]);
        }

        foreach (var r in hackDisplayList)
        {
            r.AttemptHack(hackCounterList);
        }
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

    protected override void DisplayCardList(float yOffset = 0)
    {
        base.DisplayCardList(-Screen.height/2 + cardPrefab.GetComponent<RectTransform>().rect.height * 0.25f);
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

    /// <summary>
    /// Subscribe this to the combat manager, enable and disable the shield button when invoke differently.
    /// </summary>
    public void SubscribeCombatManager()
    {
        CombatManager.GetInstance().onPlayerTurn += EnableShield;
    }

    /// <summary>
    /// Enable the ability for player to use shield
    /// </summary>
    void EnableShield(bool enableShield)
    {
        virusShieldButton.interactable = enableShield;
        wormShieldButton.interactable = enableShield;
        trojanShieldButton.interactable = enableShield;
    }

    /// <summary>
    /// Remove the counter from the list according to the list provided
    /// </summary>
    public void HackExecuted(List<CardType> cardTypeList)
    {
        foreach(CardType cardType in cardTypeList)
        {
            hackCounterList.Remove(cardType);
            GameObject removedIcon = iconDisplayList[0];
            iconDisplayList.Remove(removedIcon);
            Destroy(removedIcon);
        }
    }
    
    private void OnDestroy()
    {
        CombatManager.GetInstance().onPlayerTurn -= EnableShield;
    }

    /// <summary>
    /// Apply the hack effect according to the level and the hack effect inputted.
    /// If the lvl is 0, ignore this whole function.
    /// </summary>
    void ExecuteHackEffect(HackType hackType, int hackLvl)
    {
        if (hackLvl == 0)
            return;

        HackTypeSO hack = hm.GetHackTypeSO(hackType, hackLvl);

        switch (hackType)
        {
            case HackType.More_Nexus_Core:
                currentNexusCoreAmount += hack.amount;
                UpdateNexusCoreDisplay();
                break;
        }
    }
}
