using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    public static EnemyManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    [System.Serializable]
    private class EnemyInfo
    {
        public EnemySO enemySO;
        public GameObject enemyPrefab;
    }

    [SerializeField] List<EnemyInfo> enemyList = new List<EnemyInfo>();

    /// <summary>
    /// Get the enemyPrefab according to the enemySo given
    /// </summary>
    public GameObject GetEnemyPrefabViaEnemySO(EnemySO enemySO)
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (enemySO == enemyList[i].enemySO)
            {
                return enemyList[i].enemyPrefab;
            }
        }
        return null;
    }
}
