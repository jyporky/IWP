using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
    private static GameplayManager instance;
    public static GameplayManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    [Header("Player Stats UI Display")]
    [SerializeField] Slider healthBarSlider;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] Slider energyPointBarSlider;
    [SerializeField] TextMeshProUGUI energyPointValue;

    [Header("UI Display Reference")]
    [SerializeField] GameObject combatUIPrefab;
    [SerializeField] GameObject shopUIPrefab;
    private Transform gameplayUISpawnArea;

    [Header("Currency UI Display")]
    [SerializeField] TextMeshProUGUI gearPartsAmountText;


    private void Start()
    {
        gameplayUISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
        UpdatePlayerStatsDisplay();
    }

    /// <summary>
    /// Create the combatUI as well as its manager from the prefab.
    /// </summary>
    public void CreateUI(PathType whichpathType)
    {
        GameObject uiToReferenceTo = null;
        switch (whichpathType)
        {
            case PathType.ENEMY:
                uiToReferenceTo = combatUIPrefab;
                break;
            case PathType.SHOP:
                uiToReferenceTo = shopUIPrefab;
                break;
        }

        Instantiate(uiToReferenceTo, gameplayUISpawnArea);
    }

    /// <summary>
    /// Update the player stats display.
    /// </summary>
    public void UpdatePlayerStatsDisplay()
    {
        int currHp = PlayerManager.GetInstance().GetCurrentHealth();
        int maxHp = PlayerManager.GetInstance().GetMaxHP();
        int currEP = PlayerManager.GetInstance().GetCurrentEP();
        int maxEP = PlayerManager.GetInstance().GetMaxEP();
        healthBarSlider.value = (float)currHp / maxHp;
        healthValue.text = currHp + "/" + maxHp;
        energyPointBarSlider.value = (float)currEP / maxEP;
        energyPointValue.text = currEP + "/" + maxEP;
        gearPartsAmountText.text = PlayerManager.GetInstance().GetGearPartsAmount().ToString();
    }
}
