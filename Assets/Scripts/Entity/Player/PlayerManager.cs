using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    private PlayerInfo playerInfo;

    [SerializeField] private List<CardSO> starterCardList;
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
            playerInfo = new PlayerInfo(10, 3, 0);

            for (int i = 0; i < starterCardList.Count; i++)
            {
                playerInfo.AddToListOfCards(starterCardList[i]);
            }
        }
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
}
