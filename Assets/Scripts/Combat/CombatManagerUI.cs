using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class CombatManagerUI : MonoBehaviour
{
    private static CombatManagerUI instance;

    public static CombatManagerUI GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    [Header("Card OffSet between each card")]
    [SerializeField] float xOffSetMargin = 200; // Define the min/max x margin. Use +ve pls
    [SerializeField] float yOffSetMargin = 40; // Define the min/max y margin. Use +ve pls
    [SerializeField] float zRotationOffSetMargin = 10; // Define the min/max margin. Use +ve pls

    [Header("Shared variables")]
    [SerializeField] GameObject statusPrefab;
    [SerializeField] GameObject selectedCardPrefab;

    // The Player UI Reference
    [Header("Player Prefab Reference")]
    [SerializeField] GameObject playerCardPrefab;
    [SerializeField] Transform playerCardSpawnArea;

    [Header("Player DeckAndDiscard")]
    [SerializeField] TextMeshProUGUI playerDeckAmt;
    [SerializeField] TextMeshProUGUI playerDiscardAmt;

    [Header("Player StatusEffect")]
    [SerializeField] Transform playerStatusHolder;

    [Header("Player Nexus Core Point Reference")]
    [SerializeField] TextMeshProUGUI playerCurrentNexusCoreText;
    [SerializeField] TextMeshProUGUI playerMaximumNexusCoreText;

    [Header("Player Stats References")]
    [SerializeField] Slider playerHealthBarSlider;
    [SerializeField] TextMeshProUGUI playerHealthValue;
    [SerializeField] Slider playerEnergyPointBarSlider;
    [SerializeField] TextMeshProUGUI playerEnergyPointValue;

    [Header("Player Deck List Reference")]
    [SerializeField] GameObject deckListUIPrefab;
    [SerializeField] Button drawListButton;
    [SerializeField] Button discardListButton;

    [Header("Player Shield Reference")]
    [SerializeField] Button virusShieldButton;
    [SerializeField] Button wormShieldButton;
    [SerializeField] Button trojanShieldButton;

    [Header("Hack Information")]
    [SerializeField] GameObject hackDisplayPrefab;
    [SerializeField] Transform hackListTransform;

    [Header("Hacking Counter")]
    [SerializeField] GameObject cardTypeDisplayPrefab;
    [SerializeField] Transform cardTypeIconListReference;


    // The Enemy UI Reference
    [Header("Enemy Prefab Reference")]
    [SerializeField] GameObject enemyCardPrefab;
    [SerializeField] Transform enemyCardSpawnArea;
    [SerializeField] Transform enemyCardPlayArea;

    [Header("Enemy DeckAndDiscard")]
    [SerializeField] TextMeshProUGUI enemyDeckAmt;
    [SerializeField] TextMeshProUGUI enemyDiscardAmt;

    [Header("Enemy StatusEffect")]
    [SerializeField] Transform enemyStatusHolder;

    [Header("Enemy Nexus Core Point Reference")]
    [SerializeField] TextMeshProUGUI enemyCurrentNexusCoreText;
    [SerializeField] TextMeshProUGUI enemyMaximumNexusCoreText;

    [Header("Enemy Stats References")]
    [SerializeField] Slider enemyHealthBarSlider;
    [SerializeField] TextMeshProUGUI enemyHealthValue;
    [SerializeField] Slider enemyEnergyPointBarSlider;
    [SerializeField] TextMeshProUGUI enemyEnergyPointValue;

    [Header("Enemy Information")]
    [SerializeField] Image enemySprite;
    [SerializeField] TextMeshProUGUI enemyNameDisplay;

    [Header("Enemy Perk Information")]
    [SerializeField] Transform perkSpawnArea;

    List<CardBase> playerCardList = new List<CardBase>();
    List<CardBase> enemyCardList = new List<CardBase>();

    // Store the reference to the player and enemy
    private Entity player;
    private Entity enemy;

    // Other references
    private Transform UISpawnArea;
    private AssetManager am;

    private void Start()
    {
        am = AssetManager.GetInstance();
        UISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
        drawListButton.onClick.AddListener(OpenDrawPileList);
        discardListButton.onClick.AddListener(OpenDiscardPileList);

        virusShieldButton.onClick.AddListener(delegate { SetUpPlayerBlock(CardType.Virus); });
        wormShieldButton.onClick.AddListener(delegate { SetUpPlayerBlock(CardType.Worm); });
        trojanShieldButton.onClick.AddListener(delegate { SetUpPlayerBlock(CardType.Trojan); });
    }

    /// <summary>
    /// Set the player and the enemy entity Reference.
    /// </summary>
    public void SetPlayerAndEntity(Entity playerRef, Entity enemyRef)
    {
        player = playerRef;
        enemy = enemyRef;
    }

    /// <summary>
    /// Create a card and add it to the playerCardGO (If from player) or enemyCardGO (If from enemy).
    /// </summary>
    public void CreateCard(Entity entityRef, CardSO cardRef)
    {
        if (entityRef == player)
        {
            CardBase card = Instantiate(playerCardPrefab, playerCardSpawnArea).GetComponent<CardBase>();
            card.UpdateCardDetails(cardRef);
            playerCardList.Add(card);
            DisplayCardList(entityRef);
        }
        else if (entityRef == enemy)
        {
            CardBase card = Instantiate(enemyCardPrefab, enemyCardSpawnArea).GetComponent<EnemyCard>();
            card.UpdateCardDetails(cardRef);
            enemyCardList.Add(card);
            DisplayCardList(entityRef);
        }
    }

    /// <summary>
    /// Delete a card from the playerCardGO (If from player) or from the enemyCardGO (If from enemy).
    /// </summary>
    public void DeleteCard(Entity entityRef, CardSO cardRef)
    {
        if (entityRef == player)
        {
            for (int i = 0; i < playerCardList.Count; i++)
            {
                if (cardRef == playerCardList[i].GetCardSO())
                {
                    CardBase cardToRemove = playerCardList[i];
                    playerCardList.Remove(playerCardList[i]);
                    Destroy(cardToRemove.gameObject);
                    DisplayCardList(entityRef);
                    break;
                }
            }
        }
        else if (entityRef == enemy)
        {
            for (int i = 0; i < enemyCardList.Count; i++)
            {
                if (cardRef == enemyCardList[i].GetCardSO())
                {
                    CardBase cardToRemove = enemyCardList[i];
                    enemyCardList.Remove(enemyCardList[i]);
                    Destroy(cardToRemove.gameObject);
                    DisplayCardList(entityRef);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Rearrange the cards to make it more beautiful
    /// </summary>
    public void DisplayCardList(Entity entityRef)
    {
        List<CardBase> cardList = new List<CardBase>();
        float yOffset = 0;

        if (entityRef == player)
        {
            cardList = playerCardList;
            yOffset = -Screen.height / 2 + playerCardPrefab.GetComponent<RectTransform>().rect.height * 0.25f;
        }

        else if (entityRef == enemy)
        {
            cardList = enemyCardList;
        }

        int totalCard = cardList.Count;

        float xDistanceBetweenInterval = xOffSetMargin * 2 / (totalCard + 1);
        float yDistanceBetweenInterval = yOffSetMargin / ((totalCard + 2) / 2);

        float rotationBetweenInterval = zRotationOffSetMargin * 2 / (totalCard + 1);

        float xPos = -xOffSetMargin;
        float yPos = -yOffSetMargin;
        float zRot = zRotationOffSetMargin;

        for (int i = 0; i < totalCard; i++)
        {
            // move to the next interval and then assign the position and rotation accordingly
            xPos += xDistanceBetweenInterval;
            zRot -= rotationBetweenInterval;

            // if the i value is below half of total, increase the yPos value, otherwise decrease it instead
            if (i > totalCard / 2)
            {
                yPos -= yDistanceBetweenInterval;
            }
            else if (i < totalCard / 2)
            {
                yPos += yDistanceBetweenInterval;
            }
            else if (totalCard % 2 != 0)
            {
                yPos = 0;
            }

            cardList[i].transform.localPosition = new Vector3(xPos, yPos + yOffset, cardList[i].transform.localPosition.z);
            cardList[i].transform.eulerAngles = new Vector3(0, 0, zRot);
        }
    }

    /// <summary>
    /// Update the display for the amount of cards in the deck and the discard pile.
    /// </summary>
    public void UpdateDeckAndDiscardAmountDisplay(Entity entityRef, int entityCardsInDeck, int entityCardsInDiscard)
    {
        if (entityRef == player)
        {
            playerDeckAmt.text = entityCardsInDeck.ToString();
            playerDiscardAmt.text = entityCardsInDiscard.ToString();
        }

        else if (entityRef == enemy)
        {
            enemyDeckAmt.text = entityCardsInDeck.ToString();
            enemyDiscardAmt.text = entityCardsInDiscard.ToString();
        }
    }

    /// <summary>
    /// Update the nexus core display for the entity.
    /// </summary>
    public void UpdateNexusCoreDisplay(Entity entityRef, int entityCurrentNexus, int entityMaxNexus)
    {
        if (entityRef == player)
        {
            playerCurrentNexusCoreText.text = entityCurrentNexus.ToString();
            playerMaximumNexusCoreText.text = entityMaxNexus.ToString();

            foreach (CardBase card in playerCardList)
            {
                card.UpdatePlayableState(entityCurrentNexus);
            }
        }
        else if (entityRef == enemy)
        {
            enemyCurrentNexusCoreText.text = entityCurrentNexus.ToString();
            enemyMaximumNexusCoreText.text = entityMaxNexus.ToString();

            foreach (CardBase card in playerCardList)
            {
                card.UpdatePlayableState(entityCurrentNexus);
            }
        }
    }

    /// <summary>
    /// Update the health display of the player health (If from player) or the enemy health (If from enemy).
    /// </summary>
    public void UpdateHealthDisplay(Entity entityRef, int entityCurrentHealth, int entityMaxHealth)
    {
        if (entityRef == player)
        {
            playerHealthBarSlider.value = (float)entityCurrentHealth / entityMaxHealth;
            playerHealthValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString(); 
        }

        else if (entityRef == enemy)
        {
            enemyHealthBarSlider.value = (float)entityCurrentHealth / entityMaxHealth;
            enemyHealthValue.text = entityCurrentHealth.ToString() + "/" + entityMaxHealth.ToString();
        }
    }

    /// <summary>
    /// Update the energy display of the player energy (If from player) or the enemy energy (If from enemy).
    /// </summary>
    public void UpdateEnergyDisplay(Entity entityRef, int entityCurrentEnergy, int entityMaxEnergy)
    {
        if (entityRef == player)
        {
            playerEnergyPointBarSlider.value = (float)entityCurrentEnergy / entityMaxEnergy;
            playerEnergyPointValue.text = entityCurrentEnergy.ToString() + "/" + entityMaxEnergy.ToString();
        }

        else if (entityRef == enemy)
        {
            if (entityMaxEnergy == 0)
            {
                enemyEnergyPointBarSlider.value = 0;
                enemyEnergyPointValue.text = "0";
            }
            else
            {
                enemyEnergyPointBarSlider.value = (float)entityCurrentEnergy / entityMaxEnergy;
                enemyEnergyPointValue.text = entityCurrentEnergy.ToString() + "/" + entityMaxEnergy.ToString();
            }
        }
    }

    /// <summary>
    /// Instantiate the status effect in the player (If from player) or in the enemy (If from enemy) <br/>
    /// Return the statusEffect Instance.
    /// </summary>
    public StatusEffect CreateStatusEffect(Entity entityRef)
    {
        Transform entitySpawnArea = null;

        if (entityRef == player)
            entitySpawnArea = playerStatusHolder;

        else if (entityRef == enemy)
            entitySpawnArea = enemyStatusHolder;

        if (entitySpawnArea == null)
            return null;

        StatusEffect newStatusEffect = Instantiate(statusPrefab, entitySpawnArea).GetComponent<StatusEffect>();

        return newStatusEffect;
    }

    /// <summary>
    /// Execute the player block effect according to what effect it is.
    /// </summary>
    void SetUpPlayerBlock(CardType blockType)
    {
        player.DoBlockEffect(blockType);
    }

    /// <summary>
    /// Open and display the list of cards in the player draw pile.
    /// </summary>
    void OpenDrawPileList()
    {
        DeckList newDrawListPanel = Instantiate(deckListUIPrefab, UISpawnArea).GetComponent<DeckList>();
        newDrawListPanel.SetupDeckList(player.GetCardList(ModifyByAmountOfCardsType.BY_CARDS_IN_DRAW_PILE), "Draw Pile");
    }

    /// <summary>
    /// Open and display the list of cards in the player discard pile.
    /// </summary>
    void OpenDiscardPileList()
    {
        DeckList newDiscardListPanel = Instantiate(deckListUIPrefab, UISpawnArea).GetComponent<DeckList>();
        newDiscardListPanel.SetupDeckList(player.GetCardList(ModifyByAmountOfCardsType.BY_CARDS_IN_DISCARDS), "Discard Pile");
    }

    /// <summary>
    /// Create the list of hacks that the player have.
    /// </summary>
    public List<HackDisplay> CreatePlayerHackDisplay()
    {
        // This list stores the list of hacks the player owned
        List<HackDisplay> listOfPlayerHacksDisplay = new List<HackDisplay>();
        // This is a temp list that store the list of player hacks with hackType and leverl.
        List<HackType> listOfPlayerHacks = PlayerManager.GetInstance().GetListOfHackType();

        foreach (HackType hacks in listOfPlayerHacks)
        {
            HackDisplay newHackDisplay = Instantiate(hackDisplayPrefab, hackListTransform).GetComponent<HackDisplay>();
            newHackDisplay.SetHackType(hacks, player as Player);
            listOfPlayerHacksDisplay.Add(newHackDisplay);
        }

        return listOfPlayerHacksDisplay;
    }

    /// <summary>
    /// Add to the player hack counter depending on the card type given. Not be added if the cardType inputted is None.
    /// </summary>
    public void AddToHackCounter(CardType cardType)
    {
        Image iconCounter = Instantiate(cardTypeDisplayPrefab, cardTypeIconListReference).GetComponent<Image>();
        iconCounter.sprite = am.GetCardSprite(cardType);
    }

    /// <summary>
    /// Clear the hack counter according to the amount stated. If an amount is stated and within the list, remove the first stated value.
    /// </summary>
    public void ClearHackCounter(int amount = 0)
    {
        Image[] iconList = cardTypeIconListReference.GetComponentsInChildren<Image>();

        if (amount == 0 || amount >= iconList.Length)
        {
            for (int i = iconList.Length - 1; i >= 1; i--)
            {
                Destroy(iconList[i].gameObject);
            }
        }

        else
        {
            for (int i = 1; i <= amount; i++)
            {
                Destroy(iconList[i].gameObject);
            }
        }
    }

    /// <summary>
    /// Enable the ability for player to use shield. Set the boolean to false to disable shield.
    /// </summary>
    public void EnableShield(bool enableShield)
    {
        virusShieldButton.interactable = enableShield;
        wormShieldButton.interactable = enableShield;
        trojanShieldButton.interactable = enableShield;
    }

    /// <summary>
    /// Get the image reference of the enemy.
    /// </summary>
    /// <returns></returns>
    public Image GetEnemyImageReference()
    {
        return enemySprite;
    }

    /// <summary>
    /// Update the enemy display, needing the enemy sprite and enemy name display.
    /// </summary>
    public void UpdateEnemyDisplay(Sprite enemySpriteRef, string enemyNameRef)
    {
        enemySprite.sprite = enemySpriteRef;
        enemyNameDisplay.text = enemyNameRef;
    }

    Coroutine enemyPlayingCard;
    Coroutine enemyDying;
    GameObject enemyActiveCard;

    /// <summary>
    /// Display the card that is played by the enemy. Can adjust the position and whether it will be active or inactive.
    /// </summary>
    public void StartEnemyPlay(CardSO cardRef)
    {
        if (enemyPlayingCard == null)
            enemyPlayingCard = StartCoroutine(EnemyPlayCard(cardRef, enemyCardPlayArea));
    }

    /// <summary>
    /// Do the animation of the enemy playing their cards. (Might be temporary)
    /// </summary>
    IEnumerator EnemyPlayCard(CardSO cardPlayed, Transform playArea)
    {
        enemyActiveCard = Instantiate(selectedCardPrefab, playArea);
        enemyActiveCard.SetActive(false);
        yield return new WaitForSeconds(2);
        enemyActiveCard.transform.position = playArea.position;
        enemyActiveCard.GetComponent<CardBase>().UpdateCardDetails(cardPlayed);
        enemy.PlayCard(cardPlayed);
        enemyActiveCard.SetActive(true);
        yield return new WaitForSeconds(2);
        Destroy(enemyActiveCard);
        enemyPlayingCard = null;
        (enemy as EnemyBase)?.ExecuteTurn();
    }

    /// <summary>
    /// Force stop the enemy playing card courotine animation.
    /// </summary>
    public void ForceStopEnemyPlayingCard()
    {
        if (enemyPlayingCard != null)
            StopCoroutine(enemyPlayingCard);

        enemyPlayingCard = null;
    }

    /// <summary>
    /// Play the enemy dying animation.
    /// </summary>
    public void StartEnemyDieAnimation()
    {
        if (enemyDying == null)
            enemyDying = StartCoroutine(PlayEnemyDying());
    }

    IEnumerator PlayEnemyDying()
    {
        int iteration = 7;
        float baseBlinkTimer = 0.2f;
        float blinkTimer = baseBlinkTimer;
        float changeBy = -1;
        CanvasGroup enemySpriteCG = enemySprite.GetComponent<CanvasGroup>();

        while (iteration != 0)
        {
            blinkTimer += changeBy * Time.deltaTime;

            if (blinkTimer <= 0)
            {
                changeBy = 1;
                iteration--;
            }
            else if (blinkTimer >= baseBlinkTimer)
            {
                changeBy = -1;
                iteration--;
            }

            enemySpriteCG.alpha = Mathf.Lerp(0, 1, blinkTimer / baseBlinkTimer);
            yield return null;
        }

        (enemy as EnemyBase).EnemyDefeat();
    }

    /// <summary>
    /// Create the enemy perk Ui and display it.
    /// </summary>
    public void DisplayEnemyPerk(Sprite perkSprite, string perkDescription)
    {
        EnemyPerkDisplay enemyPerk = Instantiate(am.GetPerkPrefab(), perkSpawnArea).GetComponent<EnemyPerkDisplay>();
        enemyPerk.UpdatePerkDisplay(perkSprite, perkDescription);
    }

    /// <summary>
    /// Delete all card in the card List of player and enemy. <br/>
    /// If any courotine card is created, destroy it. <br/>
    /// If any status effect is created, destroy it. <br/>
    /// If there is any existing hack display, destroy it. <br/>
    /// If there is any existing hack Icon, destroy it. <br/>
    /// If there is any perk display, destroy it.
    /// </summary>
    public void RefreshAll()
    {
        for (int i = playerCardList.Count - 1; i >= 0; i--)
        {
            Destroy(playerCardList[i].gameObject);
        }

        for (int i = enemyCardList.Count - 1; i >= 0; i--)
        {
            Destroy(enemyCardList[i].gameObject);
        }

        playerCardList.Clear();
        enemyCardList.Clear();

        if (enemyActiveCard != null)
            Destroy(enemyActiveCard);

        StatusEffect[] playerStatusEffect = playerStatusHolder.gameObject.GetComponentsInChildren<StatusEffect>();
        for (int i = playerStatusEffect.Length - 1; i >= 0; i--)
        {
            Destroy(playerStatusEffect[i].gameObject);
        }

        StatusEffect[] enemyStatusEffect = enemyStatusHolder.gameObject.GetComponentsInChildren<StatusEffect>();
        for (int i = enemyStatusEffect.Length - 1; i >= 0; i--)
        {
            Destroy(enemyStatusEffect[i].gameObject);
        }

        HackDisplay[] hackDisplayList = hackListTransform.gameObject.GetComponentsInChildren<HackDisplay>();
        for (int i = hackDisplayList.Length - 1; i >= 0; i--)
        {
            Destroy(hackDisplayList[i].gameObject);
        }

        Image[] hackIconList = cardTypeIconListReference.gameObject.GetComponentsInChildren<Image>();
        for (int i = hackDisplayList.Length - 1; i >= 1; i--)
        {
            Destroy(hackIconList[i].gameObject);
        }

        EnemyPerkDisplay[] perkList = perkSpawnArea.gameObject.GetComponentsInChildren<EnemyPerkDisplay>();
        for (int i =  perkList.Length - 1;i >= 0;i--)
        {
            Destroy(perkList[i].gameObject);
        }
    }
}
