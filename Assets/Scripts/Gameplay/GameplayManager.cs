using System;
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
        combatUIPanel.SetActive(false);
        shopUIPanel.SetActive(false);
        upgradeStationPanel.SetActive(false);
        eventUIPanel.SetActive(false);
        chamberPanel.SetActive(true);
    }

    [Header("Player Stats UI Display")]
    [SerializeField] GameObject playerStatsUIDisplay;
    [SerializeField] Slider healthBarSlider;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] Slider energyPointBarSlider;
    [SerializeField] TextMeshProUGUI energyPointValue;

    [Header("UI Display Reference")]
    [SerializeField] GameObject combatUIPanel;
    [SerializeField] GameObject shopUIPanel;
    [SerializeField] GameObject upgradeStationPanel;
    [SerializeField] GameObject eventUIPanel;
    [SerializeField] GameObject chamberPanel;
    private GameObject[] panelArray;

    [Header("Currency UI Display")]
    [SerializeField] TextMeshProUGUI gearPartsAmountText;

    [Header("RoomDisplayList")]
    [SerializeField] TextMeshProUGUI roomClearedAmountText;   

    private void Start()
    {
        SetPanelActive(PathType.NONE);
        UpdatePlayerStatsDisplay();
    }

    /// <summary>
    /// Set the selected panel active, if the pathtype is none, set all active panel inactive and display the chamber panel.
    /// </summary>
    public void SetPanelActive(PathType whichpathType)
    {
        switch (whichpathType)
        {
            case PathType.ENEMY:
                combatUIPanel.SetActive(true);
                StartCoroutine(DelayOneFrame(result => combatUIPanel.GetComponent<CombatManager>().StartCombat()));
                chamberPanel.SetActive(false);
                TogglePlayerStatsDisplay(false);
                break;
            case PathType.SHOP:
                shopUIPanel.SetActive(true);
                StartCoroutine(DelayOneFrame(result => shopUIPanel.GetComponent<ShopManagerUI>().LoadToShop()));
                chamberPanel.SetActive(false);
                break;
            case PathType.UPGRADE_STATION:
                upgradeStationPanel.SetActive(true);
                chamberPanel.SetActive(false);
                break;
            case PathType.EVENT:
                eventUIPanel.SetActive(true);
                StartCoroutine(DelayOneFrame(result => eventUIPanel.GetComponent<EventManagerUI>().LoadToEvent()));
                chamberPanel.SetActive(false);
                break;
            case PathType.NONE:
                chamberPanel.SetActive(true);
                combatUIPanel.SetActive(false);
                shopUIPanel.SetActive(false);
                upgradeStationPanel.SetActive(false);
                eventUIPanel.SetActive(false);
                TogglePlayerStatsDisplay(true);
                break;
        }
    }

    /// <summary>
    /// Control whether the player stats display is active or not.
    /// </summary>
    void TogglePlayerStatsDisplay(bool toggle)
    {
        playerStatsUIDisplay.SetActive(toggle);
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

    public void UpdateRoomCleared(int currentRoomIndex, int totalAmountOfRooms)
    {
        roomClearedAmountText.text = currentRoomIndex.ToString() + "/" + totalAmountOfRooms.ToString() + " Rooms Cleared";
    }

    IEnumerator DelayOneFrame(Action<bool> callback)
    {
        yield return null;
        callback(true);
    }
}
