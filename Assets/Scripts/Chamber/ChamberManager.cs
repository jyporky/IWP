using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Define the room type of the room.
/// </summary>
public enum Room_Type
{
    RANDOM_ROOM,
    STARTING_ROOM,
    ENEMY_ROOM,
    NORMAL_ROOM,
    REST_ROOM,
    ELITE_ROOM,
    BOSS_ROOM,
    TOTAL,
}

[System.Serializable]
public class MinimumRoomAmount
{
    public Room_Type roomType;
    public int amount;
}

[System.Serializable]
public class RoomDisplay
{
    public Room_Type roomType;
    public Sprite roomSpriteIcon;
}

public class ChamberManager : MonoBehaviour
{
    private static ChamberManager instance;
    public static ChamberManager GetInstance()
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
            DontDestroyOnLoad(gameObject);
        }
    }

    [Header("RoomDisplayList")]
    [SerializeField] List<RoomDisplay> roomDisplayList;
    [SerializeField] GameObject roomIconPrefab;

    [Header("chamberList")]
    private List<GameObject> roomList = new List<GameObject>();
    [SerializeField] List<ChamberSO> chamberList;

    [Header("roomSelection")]
    [SerializeField] Color32 currentRoomColor;
    [SerializeField] Color32 completedRoomColor;

    // the current chamber and room information
    private int currentChamberIndex = 0;
    private int currentRoomIndex = 0;
    bool clearChamber = false;
    private PathType pathSelected;
    private EnemySO enemySelected;

    public delegate void DestroySelfAfterChamber();
    /// <summary>
    /// Register this delegate event to destroy self when loading into a new chamber
    /// </summary>
    public DestroySelfAfterChamber destroySelfAfterChamber;
    private void Start()
    {
        ShopManager.GetInstance().AddToShopList(chamberList[currentChamberIndex].avaliableShopItemList);
        List<Room_Type> loadedRoomList = chamberList[currentChamberIndex].rooms;
        Transform roomIconSpawn = GameObject.FindGameObjectWithTag("RoomIconSpawn").transform;
        Transform pathSpawnLocation = GameObject.FindGameObjectWithTag("PathObjectSpawn").transform;
        for (int i = 0; i < loadedRoomList.Count; i++)
        {
            GameObject newRoomIcon = Instantiate(roomIconPrefab, roomIconSpawn);
            Room_Type whatRoomType = loadedRoomList[i];
            if (whatRoomType == Room_Type.RANDOM_ROOM)
            {
                whatRoomType = GetRandomRoomType();
            }
            newRoomIcon.GetComponent<RoomIconDisplay>().UpdateSpriteIcon(FindSprite(whatRoomType));
            newRoomIcon.GetComponent<RoomIconDisplay>().UpdateRoomType(whatRoomType);
            GeneratePathInRoom(newRoomIcon, whatRoomType, pathSpawnLocation);
            newRoomIcon.GetComponent<RoomIconDisplay>().SetPathActive(false);
            roomList.Add(newRoomIcon);
        }
        roomList[currentRoomIndex].GetComponent<RoomIconDisplay>().ChangeColor(currentRoomColor);
        roomList[currentRoomIndex].GetComponent<RoomIconDisplay>().SetPathActive(true);
    }

    /// <summary>
    /// Generate up to 3 paths in the room according to the room type
    /// </summary>
    void GeneratePathInRoom(GameObject roomIconObject, Room_Type roomType, Transform pathSpawnArea)
    {
        List<PathType> newPathList = PathManager.GetInstance().GetPathAccordingToRoomType(roomType);
        for (int i = 0; i < newPathList.Count;i++)
        {
            GameObject pathObjectPrefab = PathManager.GetInstance().GetPathPrefab(newPathList[i]);
            GameObject newPathObject = Instantiate(pathObjectPrefab, pathSpawnArea);
            ModifyPathIfNeeded(newPathObject, newPathList[i]);
            roomIconObject.GetComponent<RoomIconDisplay>().AddPathToList(newPathObject);
        }
    }

    /// <summary>
    /// Modify the pathInfo and update the display if needed. Dev Note: Add Elite and bosses and relink later
    /// </summary>
    void ModifyPathIfNeeded(GameObject pathInfo, PathType pathType)
    {
        switch (pathType)
        {
            case PathType.ENEMY:
            case PathType.ELITE:
            case PathType.BOSS:
                List<EnemySO> enemyList = chamberList[currentChamberIndex].possibleEnemyToAppearList;
                int enemySpawnIndex = Random.Range(0, enemyList.Count);
                pathInfo.GetComponent<EnemyPath>().SetEnemy(enemyList[enemySpawnIndex]);
                break;

            case PathType.EVENT:
                break;
        }
    }

    /// <summary>
    /// Set the current room color to completedRoomColor, destroy the current room path and increase the currentroom index by 1.
    /// Afterwards, change the currentroom color to currentRoomColor as well as display the new currentroom path.
    /// </summary>
    public void ClearRoom()
    {
        if (clearChamber)
            return;

        roomList[currentRoomIndex].GetComponent<RoomIconDisplay>().ChangeColor(completedRoomColor);
        roomList[currentRoomIndex].GetComponent<RoomIconDisplay>().DestroyPaths();
        currentRoomIndex++;
        if (currentRoomIndex >= roomList.Count)
        {
            clearChamber = true;
            Debug.Log("Stop");
        }
        else
        {
            roomList[currentRoomIndex].GetComponent<RoomIconDisplay>().ChangeColor(currentRoomColor);
            roomList[currentRoomIndex].GetComponent<RoomIconDisplay>().SetPathActive(true);
        }
    }

    /// <summary>
    /// Return the sprite from the roomdisplayList which stores the sprite according to the roomtype in the parameter.
    /// </summary>
    Sprite FindSprite(Room_Type roomType)
    {
        return roomDisplayList[(int)roomType].roomSpriteIcon;
    }

    /// <summary>
    /// Returns a random room type.
    /// </summary>
    /// <returns></returns>
    Room_Type GetRandomRoomType()
    {
        int whichRoom = Random.Range(0, 3);
        switch (whichRoom)
        {
            case 0:
                return Room_Type.NORMAL_ROOM;
            case 1:
                return Room_Type.ENEMY_ROOM;
            case 2:
                return Room_Type.REST_ROOM;
            default:
                return Room_Type.NORMAL_ROOM;
        }
    }

    /// <summary>
    /// Interact with the path according to the parameter.
    /// According to the path, it will load up different UI Panel accordingly.
    /// </summary>
    public void InteractWithPath(PathType pathType)
    {
        UITransition.GetInstance().BeginTransition(result =>
        {
            SpawnPathUI();
        });

        switch (pathType)
        {
            case PathType.SHOP:
                pathSelected = PathType.SHOP;
                Debug.Log("Shop");
                break;

            case PathType.BLACKSMITH:
                pathSelected = PathType.BLACKSMITH;
                Debug.Log("Blacksmith");
                break;

            case PathType.STARTING:
                pathSelected = PathType.STARTING;
                Debug.Log("Start Journey");
                break;

            case PathType.EVENT:
                pathSelected = PathType.EVENT;
                Debug.Log("Event");
                break;

            case PathType.ENEMY:
            case PathType.ELITE:
            case PathType.BOSS:
                pathSelected = PathType.ENEMY;
                break;

        }
    }

    /// <summary>
    /// Spawn the path UI according to the path selected. Can bring up enemy, shop, blacksmith UI. Might bring up event if needed.
    /// </summary>
    void SpawnPathUI()
    {
        switch (pathSelected)
        {
            case PathType.ENEMY:
                GameplayManager.GetInstance().CreateUI(PathType.ENEMY);
                Debug.Log("Engage with battle");
                break;
            case PathType.SHOP:
                GameplayManager.GetInstance().CreateUI(PathType.SHOP);
                Debug.Log("Enter Shop");
                break;
            default:
                ClearRoom();
                break;
        }
    }
}
