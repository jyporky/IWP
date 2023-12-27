using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static CombatManager;

public class EnemyBase : Entity
{
    [Header("UI references")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] Slider shieldPointBarSlider;
    [SerializeField] TextMeshProUGUI shieldPointValue;
    [SerializeField] TextMeshProUGUI enemyName;

    [Header("Enemy stuff references")]
    [SerializeField] Transform enemyPlayCardArea;
    protected GameObject enemycardPrefab;

    [Header("Entity Information")]
    [SerializeField] Image entitySprite;
    [SerializeField] TextMeshProUGUI entityNameDisplay;

    [Header("Perk Information")]
    [SerializeField] bool havePerk = true;
    [SerializeField] Transform perkSpawnArea;
    [SerializeField] Sprite perkSprite;
    [SerializeField] string perkDescription;

    private EnemySO enemySO;
    protected int level;

    private Coroutine enemyPlayingCard;

    protected override void Awake()
    {
        base.Awake();
        // Link the cardspawnarea
        cardSpawnArea = GameObject.FindGameObjectWithTag("EnemyCardSpawn").transform;
        cardPrefab = am.GetEnemyCardPrefab(true);
        enemycardPrefab = am.GetSelectedCardPrefab();
    }

    private void Start()
    {
        ChangeHealth(0);
        ChangeShieldPoint(0);
        CombatManager.GetInstance().onEnemyPlay += ExecuteTurn;
        CombatManager.GetInstance().onGameEnd += UnSubscribeAll;
        UpdateDeckAndDiscardAmountDisplay();

        if (havePerk)
        {
            GameObject perk = Instantiate(am.GetPerkPrefab(), perkSpawnArea);
            perk.GetComponent<EnemyPerkDisplay>().UpdatePerkDisplay(perkSprite, perkDescription);
        }
    }

    /// <summary>
    /// Load the enemy stats to the Entity details as well as their deck
    /// </summary>
    public void LoadStatsAndDeck(EnemySO enemySOInfo)
    {
        enemySO = enemySOInfo;
        maxHP = enemySOInfo.enemyHP;
        currentHP = maxHP;
        maxSP = enemySOInfo.enemySP;
        currentSP = maxSP;
        level = enemySOInfo.level;
        entitySprite.sprite = enemySOInfo.enemySprite;
        entityNameDisplay.text = enemySOInfo.enemyName + "(lv " + enemySOInfo.level + ")";
        List<CardSO> cardList = enemySOInfo.enemyDeck;

        for (int i = 0; i < cardList.Count; i++)
        {
            cardsInDeckList.Add(cardList[i]);
        }
    }

    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);
        healthBarSlider.value = (float)currentHP / maxHP;
        healthValue.text = currentHP.ToString() + "/" + maxHP.ToString();
        if (currentHP <= 0)
        {
            CombatManager.GetInstance().StopGame();
            StartCoroutine(EnemyDefeated());
        }
    }

    public override void ChangeShieldPoint(int shieldPointChanged)
    {
        base.ChangeShieldPoint(shieldPointChanged);

        if (maxSP == 0)
        {
            shieldPointBarSlider.value = 0;
            shieldPointValue.text = "0";
        }
        else
        {
            shieldPointBarSlider.value = (float)currentSP / maxSP;
            shieldPointValue.text = currentSP.ToString() + "/" + maxSP.ToString();
        }
    }

    /// <summary>
    /// Make the AI do something.
    /// </summary>
    void ExecuteTurn()
    {
        if (cardsInHandList.Count > 0)
        {
            int cardToPlay = Random.Range(0, cardsInHandList.Count);
            CardSO cardPlayed = cardsInHandList[cardToPlay];

            if (enemyPlayingCard == null)
            {
                enemyPlayingCard = StartCoroutine(EnemyPlayCard(cardPlayed, enemyPlayCardArea));
            }
        }
        else
        {
            CombatManager.GetInstance().EndEnemyTurn();
        }
    }

    /// <summary>
    /// Force the enemy to stop playing their cards.
    /// </summary>
    public void ForceStopPlayingCard()
    {
        if (enemyPlayingCard != null)
            StopCoroutine(enemyPlayingCard);
    }

    /// <summary>
    /// Do the animation of the enemy playing their cards. (Might be temporary)
    /// </summary>
    IEnumerator EnemyPlayCard(CardSO cardPlayed, Transform playArea)
    {
        GameObject enemyActiveCard = Instantiate(enemycardPrefab, gameObject.transform);
        enemyActiveCard.SetActive(false);
        yield return new WaitForSeconds(2);
        enemyActiveCard.transform.position = playArea.position;
        enemyActiveCard.GetComponent<SelectedCard>().UpdateCardDetails(cardPlayed);
        base.PlayCard(cardPlayed);
        enemyActiveCard.SetActive(true);
        yield return new WaitForSeconds(2);
        Destroy(enemyActiveCard);
        enemyPlayingCard = null;
        ExecuteTurn();
    }

    /// <summary>
    /// Play some animation of the enemy being defeated
    /// </summary>
    /// <returns></returns>
    IEnumerator EnemyDefeated()
    {
        int iteration = 7;
        float baseBlinkTimer = 0.2f;
        float blinkTimer = baseBlinkTimer;
        float changeBy = -1;
        CanvasGroup enemySpriteCG = entitySprite.GetComponent<CanvasGroup>();

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

            enemySpriteCG.alpha = Mathf.Lerp(0, 1, blinkTimer/ baseBlinkTimer);
            yield return null;
        }

        CombatManager.GetInstance().SetGameOver(false);
        CombatManager.GetInstance().onEnemyPlay -= ExecuteTurn;
        Destroy(gameObject);
    }

    /// <summary>
    /// Unsubscribe from all invoke functions.
    /// </summary>
    void UnSubscribeAll()
    {
        CombatManager.GetInstance().onEnemyPlay -= ExecuteTurn;
        CombatManager.GetInstance().onGameEnd -= UnSubscribeAll;
    }
}
