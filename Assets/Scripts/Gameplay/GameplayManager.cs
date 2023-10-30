using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] GameObject playerGameplayReference;
    [SerializeField] GameObject selectedCardPrefab;

    [Header("Object Reference")]
    [SerializeField] Transform selectedCardSpawnTransform;
    [SerializeField] Transform playerArea;
    [SerializeField] Transform enemyArea;
    [SerializeField] EnemySO tempEnemySOReference;
    [SerializeField] Button endTurnButton;
    [SerializeField] Button reshuffledDeckButton;

    [Header("Gameover Screen")]
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] TextMeshProUGUI gameOverTitleDisplay;

    [Header("Enemy Left")]
    [SerializeField] string baseEnemyLeftDisplayText;
    [SerializeField] TextMeshProUGUI enemyLeftCounterDisplay;

    private GameObject selectedCard;
    Coroutine draggingCards;

    public delegate void OnDragging(bool changeIsSelected);
    public OnDragging onDragging;
    private GameObject player;
    private GameObject enemy;
    private Vector2 enemyMinHitbox;
    private Vector2 enemyMaxHitbox;
    private List<GameObject> enemyObjectReferenceList = new List<GameObject>();
    private Turn currentTurn = Turn.PLAYER_TURN;


    public delegate void OnEnemyPlay();
    /// <summary>
    /// Delegate event meant for enemy to execute their turn.
    /// </summary>
    public OnEnemyPlay onEnemyPlay;

    private static GameplayManager instance;
    public static GameplayManager GetInstance()
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
        for (int i = 0; i < 2; i++)
        {
            GameObject enemyReference = EnemyManager.GetInstance().GetEnemyPrefabViaEnemySO(tempEnemySOReference);
            GameObject newEnemy = Instantiate(enemyReference, enemyArea);
            newEnemy.name += i.ToString();
            enemyObjectReferenceList.Add(newEnemy);
            newEnemy.GetComponent<EnemyBase>().LoadStatsAndDeck(tempEnemySOReference);
            newEnemy.SetActive(false);
        }

        UpdateEnemyLeftDisplay();
        gameOverScreen.SetActive(false);
        enemy = enemyObjectReferenceList[0];
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
                        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, enemy.GetComponent<Entity>(), caster);
                    }
                    else
                    {
                        CardManager.GetInstance().ExecuteKeywordEffect(statusInfo, player.GetComponent<Entity>(), caster);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Call this function when the current enemy the player is fighting died. Change the current enemy to the next enemy in the list as well as 
    /// reshuffle the player deck. If there is no next enemy in the list, the player clear the run.
    /// </summary>
    public void CurrentEnemyDied(Entity reference)
    {
        enemyObjectReferenceList.Remove(reference.gameObject);
        if (enemyObjectReferenceList.Count > 0)
        {
            CardManager.GetInstance().ForceStopCardEffect();
            player.GetComponent<Entity>().RefreshStatusAndDeck();
            enemy = enemyObjectReferenceList[0];
            enemy.SetActive(true);
            EndEnemyTurn();
        }
        else
        {
            SetGameOver(false);
        }
        UpdateEnemyLeftDisplay();
    }

    /// <summary>
    /// Update the text display of the enemies left.
    /// </summary>
    void UpdateEnemyLeftDisplay()
    {
        enemyLeftCounterDisplay.text = baseEnemyLeftDisplayText + enemyObjectReferenceList.Count.ToString();
    }

    /// <summary>
    /// Set the game to a gameOver state. If the playerLose boolean is true, trigger run fail. Else trigger run clear.
    /// </summary>
    public void SetGameOver(bool playerLose)
    {
        gameOverScreen.SetActive(true);
        switch (playerLose)
        {
            case true:
                gameOverTitleDisplay.text = "Run Fail!";
                break;
            case false:
                gameOverTitleDisplay.text = "Run Success!";
                break;
        }
    }
}
