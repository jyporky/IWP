using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListShuffler : MonoBehaviour
{
    public class ListValue<T>
    {
        public T value;
        public int minAmt;
        public int maxAmt;
    }

    public class ListIgnore<T>
    {
        public T valueToIgnore;
        public int whichIndex;
    }

    private static ListShuffler instance;
    public static ListShuffler GetInstance()
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

    private class ListValueInfo
    {
        public int amount;
        public int maxAmount;
    }

    /// <summary>
    /// Generate a list according to the min and max amt of each object type. <br/>
    /// Putting the required amount of values is optional.
    /// </summary>
    public List<T> GenerateList<T>(List<ListValue<T>> listToUse, int requiredAmount = 0, List<ListIgnore<T>> listToExclude = null)
    {
        List<T> listToReturn = new List<T>();
        Dictionary<T, ListValueInfo> valuesAmount = new Dictionary<T, ListValueInfo>();

        // Create a new listToExclude if it does not exist. This list will be used as a specific room to follow.
        List<ListIgnore<T>> specifyList;
        switch (listToExclude == null)
        {
            case true:
                specifyList = new List<ListIgnore<T>>();
                break;
            case false:
                specifyList = new(listToExclude);
                break;
        }

        bool validList = false;

        int excludedRoomAmt = 0;
        if (listToExclude != null)
            excludedRoomAmt = specifyList.Count;

        while (!validList)
        {
            valuesAmount.Clear();
            int totalAmount = 0;
            foreach (var item in listToUse)
            {
                int extraIndex = GetHowManyExistInIgnoreList(item.value, specifyList);
                int amount = Random.Range(item.minAmt, item.maxAmt + 1 - extraIndex);
                ListValueInfo newListValueInfo = new ListValueInfo();
                newListValueInfo.amount = amount + extraIndex;
                newListValueInfo.maxAmount = item.maxAmt;
                valuesAmount.Add(item.value, newListValueInfo);
                totalAmount += amount;
            }

            if (totalAmount + excludedRoomAmt <= requiredAmount && requiredAmount != 0)
            {
                validList = true;
            }
        }

        foreach (var item in valuesAmount)
        {
            for (int i = 0; i < item.Value.amount; i++)
            {
                listToReturn.Add(item.Key);
            }
        }

        // Add any random additional value to fulfill the requiredAmount.
        while (requiredAmount > listToReturn.Count + excludedRoomAmt)
        {
            // if the item becomes 0 or lesser, remove from the key. It will not be used anymore.

            List<T> keysToRemove = new List<T>();

            foreach (var item in valuesAmount)
            {
                if (item.Value.amount >= item.Value.maxAmount)
                    keysToRemove.Add(item.Key);
            }

            foreach(var key in keysToRemove)
            {
                valuesAmount.Remove(key);
            }

            // Generate a random index, which will be use to select which value to use.
            int whichIndex = Random.Range(0, valuesAmount.Count);
            List<T> tempList = new List<T>(valuesAmount.Keys);
            // Add that value into the list to return as well as reducing the possible amount left by 1.
            listToReturn.Add(tempList[whichIndex]);
            valuesAmount[tempList[whichIndex]].amount++;
        }

        listToReturn = ReshuffleList(listToReturn);
        AddSpecifyRoom(listToReturn, specifyList);
        return listToReturn;
    }

    int GetHowManyExistInIgnoreList<T>(T value, List<ListIgnore<T>> ignoreList)
    {
        int counter = 0;
        foreach (var item in ignoreList)
        {
            if (EqualityComparer<T>.Default.Equals(value, item.valueToIgnore))
            {
                counter++;
            }
        }
        return counter;
    }

    /// <summary>
    /// Force the specify room to be at the index they need to be in.
    /// </summary>
    /// <returns></returns>
    void AddSpecifyRoom<T>(List<T> listToReshuffle, List<ListIgnore<T>> specifyRoomIfAny)
    {
        for (int i = 0; i < specifyRoomIfAny.Count; i++)
        {
            if (specifyRoomIfAny[i].whichIndex < listToReshuffle.Count)
            {
                listToReshuffle.Insert(specifyRoomIfAny[i].whichIndex, specifyRoomIfAny[i].valueToIgnore);
            }
            else
            {
                listToReshuffle.Add(specifyRoomIfAny[i].valueToIgnore);
            }
        }
    }

    /// <summary>
    /// Reshuffle the content of the list.
    /// </summary>
    public List<T> ReshuffleList<T>(List<T> listToReshuffle)
    {
        List<T> returnList = new(listToReshuffle);
        int n = returnList.Count;
        while (n > 0)
        {
            n--;
            int index = Random.Range(0, n + 1);
            T temp = returnList[index];
            returnList[index] = returnList[n];
            returnList[n] = temp;
        }

        return returnList;
    }

    //public T simnething<T>()
    //{
    //    return default(T);
    //}
}
