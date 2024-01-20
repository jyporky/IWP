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

    public override void ReshuffleDeck()
    {
        base.ReshuffleDeck();
    }

    /// <summary>
    /// Make the AI do something.
    /// </summary>
    public void ExecuteTurn()
    {
        string cardList = string.Empty;
        foreach (CardSO card in cardsInHandList)
        {
            cardList += card.cardName + ", ";
        }
        Debug.Log(cardList);

        int cardFocusIter = -1;
        CardSO cardToPlay = null;

        for (int i = 0; i < enemySO.decisionPattern.Count; i++)
        {
            bool validDecision = false;
            if (enemySO.decisionPattern[i] == DecisionType.Card_Focus)
            {
                cardFocusIter++;
                validDecision = SmartEnemyAI.GetValidDecision(cardsInHandList, DecisionType.Card_Focus, currentNexusCoreAmount, enemySO.cardFocusList[cardFocusIter]);
            }
            else
            {
                validDecision = SmartEnemyAI.GetValidDecision(cardsInHandList, enemySO.decisionPattern[i], currentNexusCoreAmount);
            }

            if (validDecision)
            {
                switch (enemySO.decisionPattern[i] == DecisionType.Card_Focus)
                {
                    case true:
                        cardToPlay = SmartEnemyAI.CardToPlay(cardsInHandList, currentNexusCoreAmount, enemySO.decisionPattern[i], enemySO.cardFocusList[cardFocusIter]);
                        break;
                    case false:
                        cardToPlay = SmartEnemyAI.CardToPlay(cardsInHandList, currentNexusCoreAmount, enemySO.decisionPattern[i]);
                        break;
                }
                break;
            }
        }

        if (cardToPlay != null)
            cm.StartEnemyPlay(cardToPlay);
        else
            CombatManager.GetInstance().EndEnemyTurn();
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
