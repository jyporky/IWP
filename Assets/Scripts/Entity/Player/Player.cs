using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : Entity
{
    private HackingManager hm;
    HackDisplay[] hackDisplayList = null;
    private List<CardType> hackCounterList = new List<CardType>();

    protected override void Awake()
    {
        base.Awake();
        hm = HackingManager.GetInstance();
    }

    private void Start()
    {
        ChangeHealth(0);
        ChangeEnergyPoint(0);

        // Get the list of hacks in the combat
        hackDisplayList = cm.CreatePlayerHackDisplay().ToArray();
    }

    private void OnEnable()
    {
        LoadStats();
        LoadCardToDeck();
        UpdateDeckAndDiscardAmountDisplay();
    }

    /// <summary>
    /// Play the selected card, add the card type into the counter.
    /// </summary>
    /// <param name="cardPlayed"></param>
    public override void PlayCard(CardSO cardPlayed)
    {
        base.PlayCard(cardPlayed);
        hackCounterList.Add(cardPlayed.cardType);
        cm.AddToHackCounter(cardPlayed.cardType);

        foreach (var r in hackDisplayList)
        {
            r.AttemptHack(hackCounterList);
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();

        // If the player is overloaded, disable the player shield.
        if (statusEffectList.ContainsKey(KeywordType.Overload))
        {
            cm.EnableShield(false);
        }

        foreach (var r in hackDisplayList)
        {
            ExecuteHackEffect(r.GetHackType(), r.GetHackLvl());
            r.ReduceDuration();
        }
    }

    public override void EndTurn()
    {
        base.EndTurn();
        hackCounterList.Clear();

        cm.ClearHackCounter();

        foreach (var r in hackDisplayList)
        {
            r.AttemptHack(hackCounterList);
        }
    }

    /// <summary>
    /// Load the deck from the playerInfo into the gameplay Player
    /// </summary>
    void LoadCardToDeck()
    {
        List<CardSO> cardList = PlayerManager.GetInstance().GetCardList();
        for (int i = 0; i < cardList.Count; i++)
        {
            cardsInDeckList.Add(cardList[i]);
        }
    }

    /// <summary>
    /// Load the player stats to the Entity details
    /// </summary>
    void LoadStats()
    {
        PlayerManager pm = PlayerManager.GetInstance();
        maxHP = pm.GetMaxHP();
        currentHP = pm.GetCurrentHealth();
        maxEP = pm.GetMaxEP();
        currentEP = pm.GetCurrentEP();
        startTurnDrawAmt = 3;
    }
    
    public override void ChangeHealth(int healthChanged)
    {
        base.ChangeHealth(healthChanged);
        if (currentHP <= 0)
        {
            CombatManager.GetInstance().SetGameOver(true);
        }
    }

    public override void ChangeEnergyPoint(int shieldPointChanged)
    {
        base.ChangeEnergyPoint(shieldPointChanged);

        // If the player has less than 0 energy, clear the hack counter, reset all hacks, disable the players ability to use shield.
        if (currentEP < 0)
        {
            foreach(var hack in hackDisplayList)
            {
                cm.EnableShield(false);
                hack.ResetHack();
                hackCounterList.Clear();
                cm.ClearHackCounter();
            }
        }    
    }

    /// <summary>
    /// Remove the counter from the list according to the list provided
    /// </summary>
    public void HackExecuted(List<CardType> cardTypeList)
    {
        foreach (CardType cardType in cardTypeList)
        {
            hackCounterList.Remove(cardType);
        }
        cm.ClearHackCounter(cardTypeList.Count);
    }

    /// <summary>
    /// Apply the hack effect according to the level and the hack effect inputted.
    /// If the lvl is 0, ignore this whole function.
    /// </summary>
    void ExecuteHackEffect(HackType hackType, int hackLvl)
    {
        if (hackLvl == 0)
            return;

        HackTypeSO hack = hm.GetHackTypeSO(hackType, hackLvl);

        switch (hackType)
        {
            case HackType.More_Nexus_Core:
                currentNexusCoreAmount += hack.amount;
                UpdateNexusCoreDisplay();
                break;
        }
    }
}
