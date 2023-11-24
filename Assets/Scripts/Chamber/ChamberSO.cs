using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chamber", menuName = "ScriptableObject/Chamber")]
public class ChamberSO : ScriptableObject
{
    public List<Room_Type> rooms;

    /// <summary>
    /// Set the minimum amount of room that the rng needs to generate at minimum
    /// </summary>
    public List<MinimumRoomAmount> minimumAmountOfRoomsList;

    /// <summary>
    /// Get the list of enemies that can spawn in this chamber
    /// </summary>
    public List<EnemySO> possibleEnemyToAppearList;

    /// <summary>
    /// Add it to the possible shop item player can encounter. Remains even when going to next chamber.
    /// </summary>
    public List<ShopItemSO> avaliableShopItemList;
}