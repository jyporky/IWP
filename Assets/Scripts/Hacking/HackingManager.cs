using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HackType
{
    More_Nexus_Core,
}

[System.Serializable]
public class HackUpgradeList
{
    public HackType hackType;
    public string hackName;
    public List<HackTypeSO> hackUpgradesList;
}

[System.Serializable]
public class HackTypeLevel
{
    public HackType hackType;
    public int level;
}

public class HackingManager : MonoBehaviour
{
    private static HackingManager instance;
    public static HackingManager GetInstance()
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
        }
    }


    [SerializeField] List<HackUpgradeList> hackUpgradeList;
    private Dictionary<HackType, List<HackTypeSO>> hackUpgradeDictionary = new Dictionary<HackType, List<HackTypeSO>>();

    private void Start()
    {
        foreach (var hack in hackUpgradeList)
        {
            hackUpgradeDictionary.Add(hack.hackType, hack.hackUpgradesList);
        }
    }

    /// <summary>
    /// Get the name of the inputted hack type.
    /// </summary>
    /// <returns></returns>
    public string GetHackTypeName(HackType hackType)
    {
        foreach (var hack in hackUpgradeList)
        {
            if (hack.hackType == hackType)
            {
                return hack.hackName;
            }
        }
        return "";
    }

    /// <summary>
    /// Return a HackTypeSO reference according to the hackType as well as the level.
    /// </summary>
    /// <returns></returns>
    public HackTypeSO GetHackTypeSO(HackType hackType, int level = 1)
    {
        if (hackUpgradeDictionary.ContainsKey(hackType))
        {
            return hackUpgradeDictionary[hackType][level - 1];
        }

        return null;
    }
}
