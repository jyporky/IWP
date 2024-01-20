using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    private PlayerInfo playerInfo;

    [SerializeField] private List<CardSO> starterCardList;
    [SerializeField] int startingMaxHealth = 10;
    [SerializeField] int startingMaxEnergy = 3;
    [SerializeField] int startingGearAmount = 0;
    public static PlayerManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            playerInfo = new PlayerInfo(startingMaxHealth, startingMaxEnergy, startingGearAmount);

            for (int i = 0; i < starterCardList.Count; i++)
            {
                playerInfo.AddToListOfCards(starterCardList[i]);
            }
        }
    }

    private void Start()
    {
        playerInfo.AddNewHackType(HackType.More_Nexus_Core, 1);
    }

    /// <summary>
    /// Get the card List from the playerInfo
    /// </summary>
    public List<CardSO> GetCardList()
    {
        return playerInfo.GetCardListFromDeck();
    }

    /// <summary>
    /// Add selectd card into the playerInfo card list
    /// </summary>
    public void AddToCardList(CardSO card)
    {
        playerInfo.AddToListOfCards(card);
    }

    /// <summary>
    /// Remove the sekected card from the playerInfo card list
    /// </summary>
    public void RemoveFromCardList(CardSO card)
    {
        playerInfo.RemoveFromListOfCards(card);
    }

    /// <summary>
    /// Get the amount of cards found in the list for the inputted card.
    /// </summary>
    public int GetCardCount(CardSO card)
    {
        int counter = 0;
        List<CardSO> cardList = playerInfo.GetCardListFromDeck();
        foreach(CardSO cards in cardList)
        {
            if (cards == card)
                counter++;
        }

        return counter;
    }

    /// <summary>
    /// Get the current health of the player.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentHealth()
    {
        return playerInfo.GetCurrentHP();
    }

    /// <summary>
    /// Get the current Energy Point of the player.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentEP()
    {
        return playerInfo.GetCurrentEP();
    }

    /// <summary>
    /// Update the current health of the player. if it exceed the max amount, set the current health to the max health amount.
    /// </summary>
    public void SetCurrentHealth(int newCurrentHealth)
    {
        int maxHP = playerInfo.GetMaxHP();

        if (newCurrentHealth > maxHP)
            playerInfo.SetCurrentHP(maxHP);
        else
            playerInfo.SetCurrentHP(newCurrentHealth);
    }

    /// <summary>
    /// Update the current Energy point of the player. if it exceed the max amount, set the current energy point to the max energy point amount.
    /// </summary>
    public void SetCurrentEnergyPoint(int newCurrentEnergyPoint)
    {
        int maxEP = playerInfo.GetMaxEP();

        if (newCurrentEnergyPoint > maxEP)
            playerInfo.SetCurrentEP(maxEP);
        else
            playerInfo.SetCurrentEP(newCurrentEnergyPoint);
    }

    /// <summary>
    /// Get the max hp of the player from the playerInfo.
    /// </summary>
    /// <returns></returns>
    public int GetMaxHP()
    {
        return playerInfo.GetMaxHP();
    }

    /// <summary>
    /// Get the increase hp of the player from the playerInfo.
    /// </summary>
    /// <returns></returns>
    public int GetIncreaseHP()
    {
        return playerInfo.GetIncreaseHP();
    }

    /// <summary>
    /// Update the increase hp of the player from the playerInfo.
    /// </summary>
    /// <returns></returns>
    public void ModifyIncreaseHP(int changeBy)
    {
        playerInfo.ModifyIncreaseHP(changeBy);
    }

    /// <summary>
    /// Get the max ep of the player from the playerInfo.
    /// </summary>
    /// <returns></returns>
    public int GetMaxEP()
    {
        return playerInfo.GetMaxEP();
    }

    /// <summary>
    /// Get the increase ep of the player of the playerInfo.
    /// </summary>
    public int SetIncreaseEP()
    {
        return playerInfo.GetIncreaseEP();
    }

    /// <summary>
    /// Update the increase ep of the player from the playerInfo.
    /// </summary>
    /// <returns></returns>
    public void ModifyIncreaseEP(int changeBy)
    {
        playerInfo.ModifyIncreaseEP(changeBy);
    }

    /// <summary>
    /// Get the amount of gear parts the player has from the playerInfo.
    /// </summary>
    public int GetGearPartsAmount()
    {
        return playerInfo.GetGearPartsAmount();
    }

    /// <summary>
    /// Change the amount of gears the player has from the playerInfo.<br/>
    /// Use -ve numbers to represent subtraction.
    /// </summary>
    public void ChangeCurrentGearAmount(int gearPartChangeAmount)
    {
        playerInfo.ChangeGearPartsAmount(gearPartChangeAmount);
    }

    /// <summary>
    /// Add a new hackType to the player. By default, the base level is 0.
    /// </summary>
    public void AddNewHackType(HackType hackType, int baseLevel = 1)
    {
        playerInfo.AddNewHackType(hackType, baseLevel);
    }

    /// <summary>
    /// Increase the level of an existing hackType the player has.
    /// </summary>
    public void IncreaseHackTypeLevel(HackType hackType, int increaseBy = 1)
    {
        for (int i = 0; i < increaseBy; i++)
        {
            playerInfo.IncreaseExistingHackTypeLevel(hackType);
        }
    }

    /// <summary>
    /// Get the max level of an existing hacktype.
    /// </summary>
    /// <param name="hackType"></param>
    /// <returns></returns>
    public int GetHackTypeMaxLevel(HackType hackType)
    {
        return playerInfo.GetHackTypeMaxLevel(hackType);
    }

    /// <summary>
    /// Get the list of existing hacktype.
    /// </summary>
    /// <returns></returns>
    public List<HackType> GetListOfHackType()
    {
        return playerInfo.GetListOfHackType();
    }

    public bool CheckIfHackTypeExist(HackType hackType)
    {
        List<HackType> listOfPlayerHackType = playerInfo.GetListOfHackType();
        foreach (HackType h in listOfPlayerHackType)
        {
            if (h == hackType)
            {
                return true;
            }
        }

        return false;
    }
}
