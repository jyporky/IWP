using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chamber", menuName = "ScriptableObject/Chamber")]
public class ChamberSO : ScriptableObject
{
    /// <summary>
    /// Declare the amount of rooms in this chamber.
    /// </summary>
    public int amountOfRooms;

    /// <summary>
    /// Use this list to determine a specific room at that room index. <br/>
    /// You need to use this to declare the elite and boss room.
    /// </summary>
    public List<RoomIndex> fixedRooms;

    /// <summary>
    /// Set the minimum and maximum amount of room that the rng needs to generate
    /// </summary>
    public List<MinMaxRoomAmount> minimumAmountOfRoomsList;

    /// <summary>
    /// Get the list of enemies that can spawn in this chamber
    /// </summary>
    public List<EnemySO> possibleEnemyToAppearList;

    /// <summary>
    /// Get the list of elites that can spawn in this chamber.
    /// </summary>
    public List<EnemySO> possibleElitesToAppearList;

    /// <summary>
    /// The boss of the chamber
    /// </summary>
    public EnemySO bossReference;

    /// <summary>
    /// Add it to the possible shop item player can encounter. Remains even when going to next chamber.
    /// </summary>
    public List<ShopItemSO> avaliableShopItemList;

    /// <summary>
    /// The possible events that the player can encounter in this chamber.
    /// </summary>
    public List<EventSO> avaliableEventList;
}