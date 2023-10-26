using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private EnemySO enemySO;
    protected int level;

    private void Awake()
    {
        // Link the cardspawnarea
        cardSpawnArea = GameObject.FindGameObjectWithTag("EnemyCardSpawn").transform;
    }

    private void Start()
    {
        ChangeHealth(0);
        ChangeShieldPoint(0);
        GameplayManager.GetInstance().onEnemyPlay += ExecuteTurn;
        UpdateDeckAndDiscardAmountDisplay();
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
        GameplayManager gm = GameplayManager.GetInstance();
        if (cardsInHandList.Count > 0)
        {
            int cardToPlay = Random.Range(0, cardsInHandList.Count);
            CardSO cardPlayed = cardsInHandList[cardToPlay];
            gm.StartCoroutine(gm.EnemyPlayCard(cardPlayed, enemyPlayCardArea));
        }
        else
        {
            gm.EndEnemyTurn();
        }
    }
}
