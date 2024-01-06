using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static CombatManager;

public class EnemyBase : Entity
{
    [Header("Perk Information")]
    [SerializeField] bool havePerk = true;
    [SerializeField] Sprite perkSprite;
    [SerializeField] string perkDescription;

    private EnemySO enemySO;
    protected int level;

    protected override void Awake()
    {
        base.Awake();
        // Link the cardspawnarea
        //cardSpawnArea = GameObject.FindGameObjectWithTag("EnemyCardSpawn").transform;
    }

    protected virtual void Start()
    {
        ChangeHealth(0);
        ChangeEnergyPoint(0);
        CombatManager.GetInstance().onEnemyPlay += ExecuteTurn;
        CombatManager.GetInstance().onGameEnd += UnSubscribeAll;
        UpdateDeckAndDiscardAmountDisplay();

        if (havePerk)
        {
            cm.DisplayEnemyPerk(perkSprite, perkDescription);
        }
    }

    /// <summary>
    /// Load the enemy stats to the Entity details as well as their deck
    /// </summary>
    public void LoadStatsAndDeck(EnemySO enemySOInfo)
    {
        startTurnDrawAmt = enemySOInfo.drawCardAmt;
        enemySO = enemySOInfo;
        maxHP = enemySOInfo.enemyHP;
        currentHP = maxHP;
        maxEP = enemySOInfo.enemySP;
        currentEP = maxEP;
        level = enemySOInfo.level;
        string enemyNameDisplay = enemySOInfo.enemyName + "(lv " + enemySOInfo.level + ")";
        cm.UpdateEnemyDisplay(enemySOInfo.enemySprite, enemyNameDisplay);
        List<CardSO> cardList = enemySOInfo.enemyDeck;

        for (int i = 0; i < cardList.Count; i++)
        {
            cardsInDeckList.Add(cardList[i]);
        }
    }

    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);
        if (currentHP <= 0)
        {
            CombatManager.GetInstance().StopGame();
            cm.StartEnemyDieAnimation();
        }
    }

    public override void ChangeEnergyPoint(int shieldPointChanged)
    {
        base.ChangeEnergyPoint(shieldPointChanged);
    }

    /// <summary>
    /// Make the AI do something.
    /// </summary>
    public void ExecuteTurn()
    {
        if (cardsInHandList.Count > 0)
        {
            int cardToPlay = Random.Range(0, cardsInHandList.Count);
            CardSO cardPlayed = cardsInHandList[cardToPlay];
            cm.StartEnemyPlay(cardPlayed);
        }
        else
        {
            CombatManager.GetInstance().EndEnemyTurn();
        }
    }

    /// <summary>
    /// Execute the code of after the enemy is defeated.
    /// </summary>
    public void EnemyDefeat()
    {
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
