using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PathType
{
    ENEMY,
    SHOP,
    UPGRADE_STATION,
    EVENT,
    STARTING,
    ELITE,
    BOSS,
    NONE,
}

public class PathManager : MonoBehaviour
{
    private static PathManager instance;
    public static PathManager GetInstance()
    {
        return instance;
    }

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

            for (int i = 0; i < pathInfoGameobjectList.Count; i++)
            {
                pathInfoGameobjectDictionary.Add(pathInfoGameobjectList[i].pathType, pathInfoGameobjectList[i].pathPrefab);
            }
        }
    }

    [System.Serializable]
    private class PathInfoGameobject
    {
        public PathType pathType;
        public GameObject pathPrefab;
    }

    [SerializeField] private List<PathInfoGameobject> pathInfoGameobjectList;
    private Dictionary<PathType, GameObject> pathInfoGameobjectDictionary = new Dictionary<PathType, GameObject>();

    /// <summary>
    /// Get the path prefab according to what the pathType parameter is.
    /// </summary>
    /// <param name="whatPath"></param>
    public GameObject GetPathPrefab(PathType whatPath)
    {
        return pathInfoGameobjectDictionary[whatPath];
    }

    /// <summary>
    /// Returns the list of path that is according to what the room type is.
    /// Different room types will have different possible paths to return
    /// </summary>
    public List<PathType> GetPathAccordingToRoomType(Room_Type roomType)
    {
        List<PathType> includedPathList = new List<PathType>();
        switch (roomType)
        {
            case Room_Type.STARTING_ROOM:
                includedPathList.Add(PathType.STARTING);
                break;

            case Room_Type.ENEMY_ROOM:
                includedPathList.Add(PathType.ENEMY);
                includedPathList.Add(PathType.ENEMY);
                includedPathList.Add(PathType.ENEMY);
                break;

            case Room_Type.NORMAL_ROOM:
                includedPathList.Add(PathType.ENEMY);
                includedPathList.Add(PathType.EVENT);

                List<PathType> tempRandomList = new List<PathType>
                {
                    PathType.UPGRADE_STATION,
                    PathType.SHOP
                };

                includedPathList.Add(GetRandomPathFromList(tempRandomList));
                break;

            case Room_Type.REST_ROOM:
                includedPathList.Add(PathType.SHOP);
                includedPathList.Add(PathType.UPGRADE_STATION);
                includedPathList.Add(PathType.EVENT);
                break;

            case Room_Type.ELITE_ROOM:
                includedPathList.Add(PathType.ELITE);
                break;

            case Room_Type.BOSS_ROOM:
                includedPathList.Add(PathType.BOSS);
                break;
        }

        return includedPathList;
    }

    /// <summary>
    /// Get a random path from the list in the parameter
    /// </summary>
    PathType GetRandomPathFromList(List<PathType> pathList)
    {
        int pathIndex = Random.Range(0, pathList.Count);
        return pathList[pathIndex];
    }
}
