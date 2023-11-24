using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CombatManager;

public class ShopManager : MonoBehaviour
{
    private static ShopManager instance;
    public static ShopManager GetInstance()
    {
        return instance;
    }

    private List<ShopItemSO> shopItemList = new List<ShopItemSO>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Clear the list of shop item stored in this script.
    /// </summary>
    public void ClearShopList()
    {
        shopItemList.Clear();
    }

    /// <summary>
    /// Add a shop list item to the shop list stored in this script.
    /// </summary>
    /// <param name="list"></param>
    public void AddToShopList(List<ShopItemSO> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            shopItemList.Add(list[i]);
        }
    }

    /// <summary>
    /// Get the shop list stored in this script.
    /// </summary>
    /// <returns></returns>
    public List<ShopItemSO> GetShopItemList()
    {
        return shopItemList;
    }
}
