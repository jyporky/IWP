using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class CombatManager : MonoBehaviour
{
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
    Coroutine draggingCards;

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
    private Turn currentTurn = Turn.PLAYER_TURN;

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
        TurnStart();
        endTurnButton.onClick.AddListener(EndPlayerTurn);
        reshuffledDeckButton.onClick.AddListener(ReshuflePlayerDeck);
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
    /// Update the selectedCard gameObject according to the CardSO and transform of the Card that calls this function.
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
    /// Begin the dragging state, disable all PointerEnter and PointerExit for all cards. Decrease the scale of the selectedGameobject and
    /// allow the selectedCard to follow the cursor.
    /// </summary>
    public void StartDrag()
    {
        onDragging?.Invoke(true);
        selectedCard.transform.localScale = new Vector3(1, 1, 1);
        draggingCards = StartCoroutine(DraggingCard());
    }

    /// <summary>
    /// End the dragging state, enable all PointerEnter and PointerExit for all cards. Stop the process of updating the card position
    /// according to the mouse position.
    /// </summary>
    public void EndDrag()
    {
        onDragging?.Invoke(false);
        StopCoroutine(draggingCards);
        draggingCards = null;
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
    /// If the card is played on the enemy, play the card.
    /// </summary>
    void CheckIfPlayCard()
    {
        Vector2 selectedCardPos = new Vector2(selectedCard.transform.position.x, selectedCard.transform.position.y);
        if ((selectedCardPos.x >= enemyMinHitbox.x && selectedCardPos.x <= enemyMaxHitbox.x) && (selectedCardPos.y >= enemyMinHitbox.y && selectedCardPos.y <= enemyMaxHitbox.y))
        {
            CardSO selectedCardSO = selectedCard.GetComponent<SelectedCard>().GetCardSO();
            player.GetComponent<Player>().PlayCard(selectedCardSO);
            CardManager.GetInstance().ExecuteCard(selectedCardSO, player.GetComponent<Entity>(), enemy.GetComponent<Entity>());
        }
    }

    /// <summary>
    /// Update the selectedCard position according to the mouse position
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
    public IEnumerator EnemyPlayCard(CardSO cardPlayed, Transform playArea)
    {
        yield return new WaitForSeconds(2);
        selectedCard.transform.position = playArea.position;
        selectedCard.GetComponent<SelectedCard>().UpdateCardDetails(cardPlayed);
        enemy.GetComponent<Entity>().PlayCard(cardPlayed);
        CardManager.GetInstance().ExecuteCard(cardPlayed, enemy.GetComponent<Entity>(), player.GetComponent<Entity>());
        selectedCard.SetActive(true);
        yield return new WaitForSeconds(2);
        selectedCard.SetActive(false);
        onEnemyPlay?.Invoke();
    }

    /// <summary>
    /// Forcefully reshuffled the player deck
    /// </summary>
    void ReshuflePlayerDeck()
    {
        player.GetComponent<Player>().ReshuffleDeck();
        EndPlayerTurn();
    }

    /// <summary>
    /// End the player turn. Call the TurnStart() function aftwards.
    /// </summary>
    public void EndPlayerTurn()
    {
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
                onDragging?.Invoke(false);
                player.GetComponent<Entity>().StartTurn();
                break;

            case Turn.ENEMY_TURN:
                endTurnButton.interactable = false;
                reshuffledDeckButton.interactable = false;
                onDragging?.Invoke(true);
                enemy.GetComponent<Entity>().StartTurn();
                onEnemyPlay?.Invoke();
                break;
        }
    }

    /// <summary>
    /// The enum that dictates whos turn it is. Depending on whether it is the player or the enemy turn,
    /// Some interactions make be locked or unlocked.
    /// </summary>
    private enum Turn
    {
        PLAYER_TURN,
        ENEMY_TURN,
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
                CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, caster, caster);
                break;

            // if it is not self inflict, find who the caster is and slot the target accordingly.
            case false:
                {
                    if (caster.gameObject.GetComponent<Player>())
                    {
                        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, player.GetComponent<Entity>(), enemy.GetComponent<Entity>());
                    }
                    else
                    {
                        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, enemy.GetComponent<Entity>(), player.GetComponent<Entity>());
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
}
