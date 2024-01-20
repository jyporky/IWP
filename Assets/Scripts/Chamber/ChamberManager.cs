using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Define the room type of the room.
/// </summary>
public enum Room_Type
{
    STARTING_ROOM,
    ENEMY_ROOM,
    NORMAL_ROOM,
    REST_ROOM,
    ELITE_ROOM,
    BOSS_ROOM,
    TOTAL,
}

[System.Serializable]
public class MinMaxRoomAmount
{
    public Room_Type roomType;
    public int minAmount;
    public int maxAmount;
}

[System.Serializable]
public class RoomIndex
{
    public Room_Type roomType;
    public int roomIndex;
}

[System.Serializable]
public class RoomInfo
{
    public List<GameObject> listOfPaths = new List<GameObject>();

    /// <summary>
    /// Set all path in the listOfPaths to active.
    /// </summary>
    public void SetAllPathActive()
    {
        foreach (GameObject path in listOfPaths)
        {
            path.SetActive(true);
        }    
    }

    /// <summary>
    /// Set all path in the listOfPaths to inactive.
    /// </summary>
    public void SetAllPathInactive()
    {
        foreach (GameObject path in listOfPaths)
        {
            path.SetActive(false);
        }
    }

    /// <summary>
    /// Destroy all the path in this list.
    /// </summary>
    public void DestroyAllPath()
    {
        for (int i = listOfPaths.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(listOfPaths[i]);
        }
    }
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

    [Header("chamberList")]
    private List<RoomInfo> roomList = new List<RoomInfo>();
    [SerializeField] List<ChamberSO> chamberList;

    [Header("roomSelection")]
    [SerializeField] Color32 currentRoomColor;
    [SerializeField] Color32 completedRoomColor;

    // the current chamber and room information
    private int currentChamberIndex = 0;
    private int currentRoomIndex = 0;
    //bool clearChamber = false;
    private PathType pathSelected;
    private bool eliteDefeated;

    private void Start()
    {
        GenerateRoomInList();
    }

    private class RoomTypeWrapper
    {
        public Room_Type roomType;
    }

    /// <summary>
    /// Generate the room in that chamber list. Will generate according to the currentChamberIndex of the chamber List.
    /// </summary>
    void GenerateRoomInList()
    {
        // Add the avaliable shop item into the shop list and clear the old one
        ShopManager.GetInstance()?.ClearShopList();
        ShopManager.GetInstance()?.AddToShopList(chamberList[currentChamberIndex].beforeEliteShopList);
        EventManager.GetInstance()?.ClearEventList();
        EventManager.GetInstance()?.AddToEventList(chamberList[currentChamberIndex].beforeEliteEventList);

        // Get the amount of rooms as well as the list of fixed rooms.
        int amountOfRooms = chamberList[currentChamberIndex].amountOfRooms;
        List <RoomIndex> loadedRoomList = chamberList[currentChamberIndex].fixedRooms;
        Transform pathSpawnLocation = GameObject.FindGameObjectWithTag("PathObjectSpawn").transform;

        //// Put the fixed rooms into a Dictionary for easier reference
        //Dictionary<int, Room_Type> fixedRoomDictionary = new Dictionary<int, Room_Type>();

        // Create a list of max and min and the value.
        List<ListShuffler.ListValue<RoomTypeWrapper>> toShuffleList = new List<ListShuffler.ListValue<RoomTypeWrapper>>();

        // for every room that appear in the minimumAmountOfRoomList, add them into the toShuffleList
        foreach (MinMaxRoomAmount value in chamberList[currentChamberIndex].minimumAmountOfRoomsList)
        {
            ListShuffler.ListValue<RoomTypeWrapper> newListValue = new ListShuffler.ListValue<RoomTypeWrapper>();
            RoomTypeWrapper rtw = new RoomTypeWrapper();
            rtw.roomType = value.roomType;

            newListValue.value = rtw;
            newListValue.minAmt = value.minAmount;
            newListValue.maxAmt = value.maxAmount;
            toShuffleList.Add(newListValue);
        }
        // Get the room required amount in this chamber.
        int roomRequired = chamberList[currentChamberIndex].amountOfRooms;

        // Create a list to store the specify room list.
        List<ListShuffler.ListIgnore<RoomTypeWrapper>> specifiedList = new List<ListShuffler.ListIgnore<RoomTypeWrapper>>();

        foreach (RoomIndex value in chamberList[currentChamberIndex].fixedRooms)
        {
            ListShuffler.ListIgnore<RoomTypeWrapper> newListValue = new ListShuffler.ListIgnore<RoomTypeWrapper>();
            RoomTypeWrapper rtw = new RoomTypeWrapper();
            rtw.roomType = value.roomType;

            newListValue.valueToIgnore = rtw;
            newListValue.whichIndex = value.roomIndex;
            specifiedList.Add(newListValue);
        }

        // Generate a list according to all the rooms avaliable, as well as adding the specify room into their correct position.
        // Shuffle the list.
        List<RoomTypeWrapper> generatedListResult = ListShuffler.GetInstance().GenerateList(toShuffleList, roomRequired, specifiedList);

        // Generate the path and add them into the room list.
        Transform pathSpawnArea = GameObject.FindGameObjectWithTag("PathObjectSpawn").transform;
        eliteDefeated = false;
        for (int i = 0; i < generatedListResult.Count; i++)
        {
            GeneratePathInRoom(generatedListResult[i].roomType, pathSpawnArea);
            if (generatedListResult[i].roomType == Room_Type.ELITE_ROOM)
            {
                eliteDefeated = true;
            }
        }
        eliteDefeated = false;
        roomList[currentRoomIndex].SetAllPathActive();
        GameplayManager.GetInstance()?.UpdateRoomCleared(currentRoomIndex, roomList.Count);
        GameplayManager.GetInstance()?.UpdateChamberDisplay(currentChamberIndex + 1);
    }

    /// <summary>
    /// Generate up to 3 paths in the room according to the room type. Add a new roomInfo into the room list.
    /// This new roomInfo stores the path created.
    /// </summary>
    void GeneratePathInRoom(Room_Type roomType, Transform pathSpawnArea)
    {
        RoomInfo newRoomInfo = new RoomInfo();

        List<PathType> newPathList = PathManager.GetInstance().GetPathAccordingToRoomType(roomType);
        for (int i = 0; i < newPathList.Count;i++)
        {
            GameObject pathObjectPrefab = PathManager.GetInstance().GetPathPrefab(newPathList[i]);
            GameObject newPathObject = Instantiate(pathObjectPrefab, pathSpawnArea);
            newPathObject.GetComponent<PathInfo>().Hidepath();
            ModifyPathIfNeeded(newPathObject, newPathList[i]);
            newRoomInfo.listOfPaths.Add(newPathObject);
        }
        newRoomInfo.SetAllPathInactive();
        roomList.Add(newRoomInfo);
    }

    /// <summary>
    /// Modify the pathInfo and update the display if needed. Dev Note: Add Elite and bosses and relink later
    /// </summary>
    void ModifyPathIfNeeded(GameObject pathInfo, PathType pathType)
    {
        switch (pathType)
        {
            case PathType.ENEMY:
                List<EnemySO> enemyList = new();
                switch (eliteDefeated)
                {
                    case false:
                        enemyList = chamberList[currentChamberIndex].possibleEnemyToAppearBeforeEliteList;
                        break;
                    case true:
                        enemyList = chamberList[currentChamberIndex].possibleEnemyToAppearAfterEliteList;
                        break;
                }

                int enemySpawnIndex = Random.Range(0, enemyList.Count);
                pathInfo.GetComponent<EnemyPath>().SetEnemy(enemyList[enemySpawnIndex]);
                break;

            case PathType.ELITE:
                List<EnemySO> eliteList = chamberList[currentChamberIndex].possibleElitesToAppearList;
                int eliteSpawnIndex = Random.Range(0, eliteList.Count);
                pathInfo.GetComponent<EnemyPath>().SetEnemy(eliteList[eliteSpawnIndex]);
                break;

            case PathType.BOSS:
                pathInfo.GetComponent<EnemyPath>().SetEnemy(chamberList[currentChamberIndex].bossReference);
                break;

            case PathType.EVENT:
                break;
        }
    }

    /// <summary>
    /// Set all previous path to inactive, increase the currentroom index by 1. Afterwards, set the current path to active afterwards. <br/>
    /// Update the room clear displayed also. If the room cleared is an elite enemy, set the shop and events to afterElite information.
    /// </summary>
    public void ClearRoom()
    {
        if (pathSelected == PathType.ELITE)
        {
            ShopManager.GetInstance()?.ClearShopList();
            ShopManager.GetInstance()?.AddToShopList(chamberList[currentChamberIndex].afterEliteShopList);
            EventManager.GetInstance()?.ClearEventList();
            EventManager.GetInstance()?.AddToEventList(chamberList[currentChamberIndex].afterEliteEventList);
            GameplayManager.GetInstance().ToggleHackAbilitySelection(true);
        }

        GameplayManager.GetInstance().SetPanelActive(PathType.NONE);
        //if (clearChamber)
        //    return;

        roomList[currentRoomIndex].SetAllPathInactive();
        currentRoomIndex++;

        GameplayManager.GetInstance().UpdateRoomCleared(currentRoomIndex, roomList.Count);

        if (currentRoomIndex < roomList.Count)
            roomList[currentRoomIndex].SetAllPathActive();

        if (currentRoomIndex >= roomList.Count)
        {
            //clearChamber = true;
            foreach (RoomInfo roomInfo in roomList)
            {
                roomInfo.DestroyAllPath();
            }
            roomList.Clear();

            //
            if (currentChamberIndex + 1 < chamberList.Count)
            {
                currentRoomIndex = 0;
                currentChamberIndex++;
                Debug.Log("Going Next Chamber");
                GenerateRoomInList();
                GameplayManager.GetInstance().ToggleHackAbilitySelection(true, true);
            }
            else
            {
                Debug.Log("Stop");
                //clearChamber = true;
            }
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
            SetPathActive();
        });

        switch (pathType)
        {
            case PathType.SHOP:
                pathSelected = PathType.SHOP;
                Debug.Log("Shop");
                break;

            case PathType.UPGRADE_STATION:
                pathSelected = PathType.UPGRADE_STATION;
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
                pathSelected = PathType.ENEMY;
                break;
            case PathType.ELITE:
                pathSelected = PathType.ELITE;
                break;
            case PathType.BOSS:
                pathSelected= PathType.BOSS;
                break;

        }
    }

    /// <summary>
    /// Spawn the path UI according to the path selected. Can bring up enemy, shop, blacksmith UI. Might bring up event if needed.
    /// </summary>
    void SetPathActive()
    {
        switch (pathSelected)
        {
            case PathType.ENEMY:
            case PathType.ELITE:
            case PathType.BOSS:
                GameplayManager.GetInstance().SetPanelActive(PathType.ENEMY);
                Debug.Log("Engage with battle");
                break;
            case PathType.SHOP:
                GameplayManager.GetInstance().SetPanelActive(PathType.SHOP);
                Debug.Log("Enter Shop");
                break;
            case PathType.UPGRADE_STATION:
                GameplayManager.GetInstance().SetPanelActive(PathType.UPGRADE_STATION);
                Debug.Log("Enter Upgrade Station");
                break;
            case PathType.EVENT:
                GameplayManager.GetInstance().SetPanelActive(PathType.EVENT);
                Debug.Log("Enter Event");
                break;
            default:
                ClearRoom();
                break;
        }
    }
}
