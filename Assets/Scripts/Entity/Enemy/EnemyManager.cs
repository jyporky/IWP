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
    private EnemySO selectedEnemy;

    /// <summary>
    /// Get the enemyPrefab according to the enemySO within the enemyManager
    /// </summary>
    public GameObject GetEnemyPrefab()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            if (selectedEnemy == enemyList[i].enemySO)
            {
                return enemyList[i].enemyPrefab;
            }
        }
        return null;
    }

    /// <summary>
    /// Set the enemySO reference that the player is going to engage.
    /// </summary>
    public void SetEnemySO(EnemySO enemySOReference)
    {
        selectedEnemy = enemySOReference;
    }

    /// <summary>
    /// Get the enemySO reference that the player is going to engage.
    /// </summary>
    /// <returns></returns>
    public EnemySO GetEnemySO()
    {
        return selectedEnemy;
    }
}
