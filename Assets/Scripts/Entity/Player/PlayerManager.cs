using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager instance;
    private PlayerInfo playerInfo;
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
            playerInfo = new PlayerInfo(10, 3);

            for (int i = 0; i < originalCardList.Count; i++)
            {
                playerInfo.AddToListOfCards(originalCardList[i]);
            }
        }
    }

    [SerializeField] List<CardSO> originalCardList;

    /// <summary>
    /// Get the card List from the playerInfo
    /// </summary>
    public List<CardSO> GetCardList()
    {
        return playerInfo.GetCardListFromDeck();
    }

    public int GetMaxHP()
    {
        return playerInfo.GetMaxHP();
    }

    public int GetMaxSP()
    {
        return playerInfo.GetMaxSP();
    }
}
