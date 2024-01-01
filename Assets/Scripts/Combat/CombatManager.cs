using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class CombatManager : MonoBehaviour
{
    /// <summary>
    /// The enum that dictates whos turn it is. Depending on whether it is the player or the enemy turn,
    /// Some interactions make be locked or unlocked.
    /// </summary>
    private enum Turn
    {
        PLAYER_TURN,
        ENEMY_TURN,
        NONE,
    }

    [Header("Object Reference")]
    [SerializeField] Transform selectedCardSpawnTransform;
    [SerializeField] Button endTurnButton;

    [Header("Player and Enemy Spawn Area")]
    [SerializeField] Transform playerArea;
    [SerializeField] Transform enemyArea;

    [Header("Gameover Screen")]
    [SerializeField] GameOverScreen gameOverScreen;

    [Header("Player and Enemy Reference")]
    [SerializeField] GameObject playerPrefab;

    [Header("Card Prefab Reference")]
    [SerializeField] GameObject selectedCardPrefab;

    // Reference to the selected card for the player and the enemy
    private GameObject selectedCard;
    private GameObject enemyCard;

    // For dragging of cards
    Coroutine draggingCards;
    public delegate void OnDragging(bool changeIsSelected);
    public OnDragging onDragging;

    // For unsubscring event
    public delegate void OnGameEnd();
    /// <summary>
    /// A delegate event that will unsubscribe any subscription made to any delegate event.
    /// This is called before destroying this gameobject.
    /// </summary>
    public OnGameEnd onGameEnd;

    // Player and entity references
    private Entity player;
    private Entity enemy;
    private Vector2 enemyMinHitbox;
    private Vector2 enemyMaxHitbox;

    // the current turn for the combat
    private Turn currentTurn;

    // Loot earned by player if enemy is successfully defeated.
    private int gearPartAmountGain;
    private int energyPointAmountGain;

    // Delegate event for enemy executing their turn
    public delegate void OnEnemyPlay();
    /// <summary>
    /// Delegate event meant for enemy to execute their turn.
    /// </summary>
    public OnEnemyPlay onEnemyPlay;

    private static CombatManager instance;
    public static CombatManager GetInstance()
    {
        return instance;
    }
    private void Awake()
    {
        instance = this;

        // create the selectedCard gameobject
        selectedCard = Instantiate(selectedCardPrefab, selectedCardSpawnTransform.transform);
        selectedCard.SetActive(false);
    }

    private void Start()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
    }

    /// <summary>
    /// Start the game.
    /// </summary>
    public void StartCombat()
    {
        RestartGame();
    }

    /// <summary>
    /// Set the min and max boundary of the enemy
    /// </summary>
    void SetEnemyHitbox()
    {
        GameObject imageRef = CombatManagerUI.GetInstance().GetEnemyImageReference().gameObject;
        float widthScale = imageRef.GetComponent<RectTransform>().rect.width / 2;
        float heightScale = imageRef.GetComponent<RectTransform>().rect.height / 2;
        Vector3 pos = imageRef.GetComponent<RectTransform>().anchoredPosition;
        enemyMinHitbox = new Vector2(pos.x - widthScale, pos.y - heightScale);
        enemyMaxHitbox = new Vector2(pos.x + widthScale, pos.y + heightScale);
    }

    /// <summary>
    /// Update the selectedCard gameObject according to the CardSO and transform of the Card that calls this function. <br/>
    /// Also Increase the scale of the selectedCard and render on top of it.
    /// </summary>
    public void SelectCard(CardSO card, RectTransform objectPos)
    {
        selectedCard.GetComponent<CardBase>().UpdateCardDetails(card);
        selectedCard.GetComponent<CardBase>().UpdatePlayableState(player.GetCurrentNexusCore());
        selectedCard.GetComponent<RectTransform>().anchoredPosition = new Vector3(objectPos.anchoredPosition.x, objectPos.anchoredPosition.y + 100, objectPos.position.z);
        selectedCard.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        selectedCard.SetActive(true);
    }

    /// <summary>
    /// Begin the dragging state, disable all PointerEnter and PointerExit for all cards. <br/>
    /// Decrease the scale of the selectedGameobject and allow the selectedCard to follow the cursor. <br/>
    /// Can only be called if its the player turn.
    /// </summary>
    public void StartDrag()
    {
        if (currentTurn != Turn.PLAYER_TURN)
            return;

        onDragging?.Invoke(true);
        selectedCard.transform.localScale = new Vector3(1, 1, 1);
        draggingCards = StartCoroutine(DraggingCard());
    }

    /// <summary>
    /// End the dragging state, enable all PointerEnter and PointerExit for all cards. <br/>
    /// Stop the process of updating the card position according to the mouse position. <br/>
    /// Can only be called if its the player turn.
    /// </summary>
    public void EndDrag()
    {
        if (currentTurn != Turn.PLAYER_TURN)
            return;

        if (draggingCards != null)
        {
            onDragging?.Invoke(false);
            StopCoroutine(draggingCards);
            draggingCards = null;
        }
    }

    /// <summary>
    /// Set the selectedCard to not active. Check to see if the card is played on top of the enemy.
    /// </summary>
    public void DeselectCard()
    {
        selectedCard.SetActive(false);
        CheckIfPlayCard();
    }

    /// <summary>
    /// If the card is played on the enemy, play the card. Only works if its your turn.
    /// </summary>
    void CheckIfPlayCard()
    {
        if (currentTurn != Turn.PLAYER_TURN)
            return;

        Vector2 selectedCardPos = new Vector2(selectedCard.GetComponent<RectTransform>().anchoredPosition.x, selectedCard.GetComponent<RectTransform>().anchoredPosition.y);
        if ((selectedCardPos.x >= enemyMinHitbox.x && selectedCardPos.x <= enemyMaxHitbox.x) && (selectedCardPos.y >= enemyMinHitbox.y && selectedCardPos.y <= enemyMaxHitbox.y))
        {
            CardSO selectedCardSO = selectedCard.GetComponent<SelectedCard>().GetCardSO();
            player.PlayCard(selectedCardSO);
        }
    }

    /// <summary>
    /// Update the selectedCard position according to the mouse position.
    /// </summary>
    IEnumerator DraggingCard()
    {
        while (true)
        {
            Vector3 mousePos = Input.mousePosition;
            selectedCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(mousePos.x - Screen.width/2, mousePos.y - Screen.height/2);
            yield return null;
        }
    }

    /// <summary>
    /// End the player turn. Call the TurnStart() function aftwards. Force any card dragging action to stop.
    /// </summary>
    public void EndPlayerTurn()
    {
        CombatManagerUI.GetInstance().EnableShield(false);
        EndDrag();
        currentTurn = Turn.ENEMY_TURN;
        player.EndTurn();
        TurnStart();
    }

    /// <summary>
    /// End the enemy turn. Call the TurnStart() function aftwards.
    /// </summary>
    public void EndEnemyTurn()
    {
        currentTurn = Turn.PLAYER_TURN;
        enemy.EndTurn();
        TurnStart();
    }

    /// <summary>
    /// Call the start turn functionality of either the player or the enemy depending on the currentTurn
    /// </summary>
    void TurnStart()
    {
        switch (currentTurn)
        {
            case Turn.PLAYER_TURN:
                endTurnButton.interactable = true;
                player.StartTurn();
                CombatManagerUI.GetInstance().EnableShield(true);
                break;

            case Turn.ENEMY_TURN:
                endTurnButton.interactable = false;
                enemy.StartTurn();
                onEnemyPlay?.Invoke();
                break;
        }
    }

    /// <summary>
    /// According to who the caster is, as well as whether it is inflict self, execute the effect by calling the cardManager.
    /// </summary>
    public void ExecuteCardFromDelay(Entity caster, Keyword statusInfo)
    {
        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, caster);
    }

    /// <summary>
    /// Set the game to a gameOver state. If the playerLose boolean is true, trigger run fail. Else trigger run clear.
    /// </summary>
    public void SetGameOver(bool playerLose)
    {
        switch (playerLose)
        {
            case true:
                gameOverScreen.DisplayScreen(GameOverScreen.ScreenType.GameOver, gearPartAmountGain, energyPointAmountGain);
                break;

            case false:
                gameOverScreen.DisplayScreen(GameOverScreen.ScreenType.CollectLoot, gearPartAmountGain, energyPointAmountGain);
                break;
        }
        StopGame();
    }

    /// <summary>
    /// Stop the game completely, prevent anything from running in background as well as disabling all interaction.
    /// </summary>
    public void StopGame()
    {
        currentTurn = Turn.NONE;
        CombatManagerUI.GetInstance().EnableShield(false);
        onDragging?.Invoke(true);

        if (draggingCards != null)
        {
            StopCoroutine(draggingCards);
            draggingCards = null;
        }

        CombatManagerUI.GetInstance().ForceStopEnemyPlayingCard();
    }

    /// <summary>
    /// Play a transition, destroy the gameobject after the transition fadesIn
    /// </summary>
    public void ReturnToChamber()
    {
        PlayerManager pm = PlayerManager.GetInstance();
        pm.SetCurrentHealth(player.GetCurrentHP());
        pm.SetCurrentEnergyPoint(player.GetcurrentEP() + energyPointAmountGain);
        pm.ChangeCurrentGearAmount(gearPartAmountGain);

        UITransition.GetInstance().BeginTransition(result =>
        {
            DestroyAllObject();
        });
    }

    /// <summary>
    /// Destroy this gameobject, as well as unregistering all subscribed object
    /// </summary>
    void DestroyAllObject()
    {
        ChamberManager.GetInstance().ClearRoom();
        onGameEnd?.Invoke();
    }

    /// <summary>
    /// Instiate all the objects needed for the game to run.
    /// </summary>
    void StartGame()
    {
        // create the player and enemy reference
        player = Instantiate(playerPrefab, playerArea).GetComponent<Entity>();
        Player p = player as Player;

        GameObject enemyReference = EnemyManager.GetInstance().GetEnemyPrefab();
        GameObject newEnemy = Instantiate(enemyReference, enemyArea);

        EnemySO enemySOReference = EnemyManager.GetInstance().GetEnemySO();
        newEnemy.GetComponent<EnemyBase>().LoadStatsAndDeck(enemySOReference);

        gearPartAmountGain = enemySOReference.gearPartsDrop;
        energyPointAmountGain = enemySOReference.energyPointDrop;
        enemy = newEnemy.GetComponent<Entity>();

        // Define the player and the enemy for the combat manager UI
        CombatManagerUI.GetInstance().SetPlayerAndEntity(player, enemy);

        // Set the hitbox of the enemy
        SetEnemyHitbox();

        CardManager.GetInstance().SetPlayerEnemyReference(player, enemy);

        StartCoroutine(DelaySettingTurn());
    }

    /// <summary>
    /// Destroy all the gameobject involved in the combat, start another game afterward.
    /// </summary>
    public void RestartGame()
    {
        Destroy(enemyCard);
        selectedCard.SetActive(false);
        onGameEnd?.Invoke();

        if (player != null)
            Destroy(player.gameObject);
        if (enemy != null)
            Destroy(enemy.gameObject);

        CombatManagerUI.GetInstance().RefreshAll();

        StartCoroutine(DelayStartGame());
    }

    IEnumerator DelayStartGame()
    {
        yield return null;

        StartGame();
    }

    IEnumerator DelaySettingTurn()
    {
        yield return null;
        currentTurn = Turn.PLAYER_TURN;
        TurnStart();
    }
}
