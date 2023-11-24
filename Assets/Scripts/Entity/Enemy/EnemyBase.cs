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
        CombatManager.GetInstance().onEnemyPlay += ExecuteTurn;
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
        CombatManager gm = CombatManager.GetInstance();
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
}
