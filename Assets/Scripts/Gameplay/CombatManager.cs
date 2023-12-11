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

    [Header("Prefab References")]
    [SerializeField] GameObject playerGameplayReference;
    [SerializeField] GameObject selectedCardPrefab;

    [Header("Object Reference")]
    [SerializeField] Transform selectedCardSpawnTransform;
    [SerializeField] Transform playerArea;
    [SerializeField] Transform enemyArea;
    [SerializeField] Button endTurnButton;
    [SerializeField] Button reshuffledDeckButton;

    [Header("Gameover Screen")]
    [SerializeField] GameOverScreen gameOverScreen;

    private GameObject selectedCard;
    private GameObject enemyCard;
    Coroutine draggingCards;
    Coroutine enemyPlayingCards;

    public delegate void OnDragging(bool changeIsSelected);
    public OnDragging onDragging;

    public delegate void OnGameEnd();
    /// <summary>
    /// A delegate event that will unsubscribe any subscription made to any delegate event.
    /// This is called before destroying this gameobject.
    /// </summary>
    public OnGameEnd onGameEnd;

    private GameObject player;
    private GameObject enemy;
    private Vector2 enemyMinHitbox;
    private Vector2 enemyMaxHitbox;
    private Turn currentTurn;

    // Loot earned by player if enemy is successfully defeated.
    private int gearPartAmountGain;
    private int energyPointAmountGain;


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
        selectedCard = Instantiate(selectedCardPrefab, reshuffledDeckButton.transform);
        selectedCard.SetActive(false);
    }

    private void Start()
    {
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        reshuffledDeckButton.onClick.AddListener(ReshufflePlayerDeck);
        StartGame();
    }

    /// <summary>
    /// Set the min and max boundary of the enemy
    /// </summary>
    void SetEnemyHitbox()
    {
        GameObject imageRef = enemy.GetComponentInChildren<Image>().gameObject;
        float widthScale = imageRef.GetComponent<RectTransform>().rect.width / 2;
        float heightScale = imageRef.GetComponent<RectTransform>().rect.height / 2;
        Vector3 pos = imageRef.transform.position;
        enemyMinHitbox = new Vector2(pos.x - widthScale, pos.y - heightScale);
        enemyMaxHitbox = new Vector2(pos.x + widthScale, pos.y + heightScale);
    }

    /// <summary>
    /// Update the selectedCard gameObject according to the CardSO and transform of the Card that calls this function. <br/>
    /// Also Increase the scale of the selectedCard and render on top of it.
    /// </summary>
    public void SelectCard(CardSO card, Transform objectPos)
    {
        selectedCard.GetComponent<SelectedCard>().UpdateCardDetails(card);
        selectedCard.transform.position = new Vector3(objectPos.position.x, objectPos.position.y + 100, objectPos.position.z);
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

        Vector2 selectedCardPos = new Vector2(selectedCard.transform.position.x, selectedCard.transform.position.y);
        if ((selectedCardPos.x >= enemyMinHitbox.x && selectedCardPos.x <= enemyMaxHitbox.x) && (selectedCardPos.y >= enemyMinHitbox.y && selectedCardPos.y <= enemyMaxHitbox.y))
        {
            CardSO selectedCardSO = selectedCard.GetComponent<SelectedCard>().GetCardSO();
            player.GetComponent<Player>().PlayCard(selectedCardSO);
            CardManager.GetInstance().ExecuteCard(selectedCardSO, player.GetComponent<Entity>());
        }
    }

    /// <summary>
    /// Update the selectedCard position according to the mouse position.
    /// </summary>
    IEnumerator DraggingCard()
    {
        while (true)
        {
            selectedCard.transform.position = Input.mousePosition;
            yield return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void EnemyPlayCard(CardSO cardPlayed, Transform playArea)
    {
        if (enemyPlayingCards != null)
        {
            StopCoroutine(enemyPlayingCards);
            enemyPlayingCards = null;
        }
        enemyPlayingCards = StartCoroutine(EnemyPlayingCard(cardPlayed, playArea));
    }

    /// <summary>
    /// Do the animation of the enemy playing their cards. (Might be temporary)
    /// </summary>
    IEnumerator EnemyPlayingCard(CardSO cardPlayed, Transform playArea)
    {
        GameObject enemyActiveCard = Instantiate(selectedCardPrefab, reshuffledDeckButton.transform);
        enemyCard = enemyActiveCard;
        enemyCard.SetActive(false);

        yield return new WaitForSeconds(2);
        enemyCard.transform.position = playArea.position;
        enemyCard.GetComponent<SelectedCard>().UpdateCardDetails(cardPlayed);
        enemy.GetComponent<Entity>().PlayCard(cardPlayed);
        CardManager.GetInstance().ExecuteCard(cardPlayed, enemy.GetComponent<Entity>());
        enemyCard.SetActive(true);
        yield return new WaitForSeconds(2);
        enemyCard.SetActive(false);
        Destroy(enemyCard);
        onEnemyPlay?.Invoke();
    }

    /// <summary>
    /// Forcefully reshuffled the player deck.
    /// </summary>
    void ReshufflePlayerDeck()
    {
        player.GetComponent<Player>().ReshuffleDeck();
        EndPlayerTurn();
    }

    /// <summary>
    /// End the player turn. Call the TurnStart() function aftwards. Force any card dragging action to stop.
    /// </summary>
    public void EndPlayerTurn()
    {
        EndDrag();
        currentTurn = Turn.ENEMY_TURN;
        player.GetComponent<Entity>().EndTurn();
        TurnStart();
    }

    /// <summary>
    /// End the enemy turn. Call the TurnStart() function aftwards.
    /// </summary>
    public void EndEnemyTurn()
    {
        currentTurn = Turn.PLAYER_TURN;
        enemy.GetComponent<Entity>().EndTurn();
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
                reshuffledDeckButton.interactable = true;
                player.GetComponent<Entity>().StartTurn();
                break;

            case Turn.ENEMY_TURN:
                endTurnButton.interactable = false;
                reshuffledDeckButton.interactable = false;
                enemy.GetComponent<Entity>().StartTurn();
                onEnemyPlay?.Invoke();
                break;
        }
    }

    /// <summary>
    /// According to who the caster is, as well as whether it is inflict self, execute the effect by calling the cardManager.
    /// </summary>
    public void ExecuteCardFromDelay(Entity caster, Keyword statusInfo)
    {
        // if it is self inflict, execute the effect to itself
        switch (statusInfo.inflictSelf)
        {
            case true:
                CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, caster);
                break;

            // if it is not self inflict, find who the caster is and slot the target accordingly.
            case false:
                {
                    if (caster.gameObject.GetComponent<Player>())
                    {
                        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, player.GetComponent<Entity>());
                    }
                    else
                    {
                        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, enemy.GetComponent<Entity>());
                    }
                }
                break;
        }
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
        onDragging?.Invoke(true);

        if (draggingCards != null)
        {
            StopCoroutine(draggingCards);
            draggingCards = null;
        }

        if (enemyPlayingCards != null)
        {
            StopCoroutine(enemyPlayingCards);
            enemyPlayingCards = null;
        }
    }

    /// <summary>
    /// Play a transition, destroy the gameobject after the transition fadesIn
    /// </summary>
    public void ReturnToChamber()
    {
        PlayerManager pm = PlayerManager.GetInstance();
        pm.SetCurrentHealth(player.GetComponent<Player>().GetCurrentHP());
        pm.SetCurrentEnergyPoint(player.GetComponent<Player>().GetCurrentSP() + energyPointAmountGain);
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
        Destroy(gameObject);
    }

    /// <summary>
    /// Instiate all the objects needed for the game to run.
    /// </summary>
    void StartGame()
    {
        // create the player and enemy reference
        player = Instantiate(playerGameplayReference, playerArea);
        // add the enemy reference into a list. Should an enemy die, the next one will be loaded in
        GameObject enemyReference = EnemyManager.GetInstance().GetEnemyPrefab();
        GameObject newEnemy = Instantiate(enemyReference, enemyArea);

        EnemySO enemySOReference = EnemyManager.GetInstance().GetEnemySO();
        newEnemy.GetComponent<EnemyBase>().LoadStatsAndDeck(enemySOReference);
        gearPartAmountGain = enemySOReference.gearPartsDrop;
        energyPointAmountGain = enemySOReference.energyPointDrop;
        enemy = newEnemy;
        enemy.SetActive(true);
        SetEnemyHitbox();

        CardManager.GetInstance().SetPlayerEnemyReference(player.GetComponent<Entity>(), enemy.GetComponent<Entity>());

        currentTurn = Turn.PLAYER_TURN;
        TurnStart();
    }

    /// <summary>
    /// Destroy all the gameobject involved in the combat, start another game afterward.
    /// </summary>
    public void RestartGame()
    {
        Destroy(enemyCard);
        selectedCard.SetActive(false);
        onGameEnd?.Invoke();
        Destroy(player);
        Destroy(enemy);
        StartCoroutine(DelayStartGame());
    }

    IEnumerator DelayStartGame()
    {
        yield return null;

        StartGame();
    }
}
