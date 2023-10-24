using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    [Header("Prefab References")]
    [SerializeField] GameObject playerGameplayReference;
    [SerializeField] GameObject selectedCardPrefab;

    [Header("Object Reference")]
    [SerializeField] Transform playerArea;
    [SerializeField] Transform enemyArea;
    [SerializeField] EnemySO tempEnemySOReference;
    [SerializeField] Button endTurnButton;

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
        selectedCard = Instantiate(selectedCardPrefab, gameObject.transform);
        selectedCard.SetActive(false);
    }

    private void Start()
    {
        // create the player and enemy reference
        player = Instantiate(playerGameplayReference, playerArea);
        // add the enemy reference into a list. Should an enemy die, the next one will be loaded in
        for (int i = 0; i < 1; i++)
        {
            GameObject enemyReference = EnemyManager.GetInstance().GetEnemyPrefabViaEnemySO(tempEnemySOReference);
            GameObject newEnemy = Instantiate(enemyReference, enemyArea);
            enemyObjectReferenceList.Add(newEnemy);
            newEnemy.GetComponent<EnemyBase>().LoadStatsAndDeck(tempEnemySOReference);
        }
        enemy = enemyObjectReferenceList[0];
        SetEnemyHitbox();

        TurnStart();
        endTurnButton.onClick.AddListener(EndPlayerTurn);
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

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    player.GetComponent<Player>().DrawCardFromDeck();
        //}
        /*else */if (Input.GetKeyDown(KeyCode.R))
        {
            player.GetComponent<Player>().ReshuffleDeck();
            EndPlayerTurn();
            Debug.Log("Deck Reshuffled");
        }
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
            CardManager.GetInstance().ExecuteCard(selectedCardSO, player.GetComponent<Player>(), enemy.GetComponent<EnemyBase>(), CastToWho.PLAYERTOENEMY);
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
        enemy.GetComponent<EnemyBase>().PlayCard(cardPlayed);
        CardManager.GetInstance().ExecuteCard(cardPlayed, player.GetComponent<Player>(), enemy.GetComponent<EnemyBase>(), CastToWho.ENEMYTOPLAYER);
        selectedCard.SetActive(true);
        yield return new WaitForSeconds(2);
        selectedCard.SetActive(false);
        onEnemyPlay?.Invoke();
    }


    /// <summary>
    /// End the player turn. Call the TurnStart() function aftwards.
    /// </summary>
    public void EndPlayerTurn()
    {
        currentTurn = Turn.ENEMY_TURN;
        TurnStart();
    }

    /// <summary>
    /// End the enemy turn. Call the TurnStart() function aftwards.
    /// </summary>
    public void EndEnemyTurn()
    {
        Debug.Log("Enemy Turn end");
        currentTurn = Turn.PLAYER_TURN;
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
                onDragging?.Invoke(false);
                for (int i = 0; i < 3; i++)
                {
                    player.GetComponent<Player>().DrawCardFromDeck();
                }
                break;

            case Turn.ENEMY_TURN:
                Debug.Log("Enemy turn starts");
                enemy.GetComponent<EnemyBase>().DrawCardFromDeck();
                enemy.GetComponent<EnemyBase>().DrawCardFromDeck();
                endTurnButton.interactable = false;
                onDragging?.Invoke(true);
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
}
