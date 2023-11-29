using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("Game Over Screen UI Reference")]
    [SerializeField] GameObject gameOverScreenPanel;
    [SerializeField] Button restartGameButton;

    [Header("Enemy Defeated Screen UI Reference")]
    [SerializeField] GameObject enemyDefeatedScreenPanel;
    [SerializeField] TextMeshProUGUI gearPartsAmountText;
    [SerializeField] TextMeshProUGUI energyPointAmountText;
    [SerializeField] Button continueButton;

    private void Start()
    {
        gameOverScreenPanel.SetActive(false);
        enemyDefeatedScreenPanel.SetActive(false);
        gameObject.SetActive(false);
        continueButton.onClick.AddListener(CollectLoot);
        restartGameButton.onClick.AddListener(RestartGame);
    }

    public enum ScreenType
    {
        GameOver,
        CollectLoot,
    }

    /// <summary>
    /// Display either the gameover screen or the collect enemy loot screen depending on the ScreenType variable inputted.
    /// <br/>
    /// If collect Enemy loot, place the gear and energy point amount here.
    /// </summary>
    public void DisplayScreen(ScreenType whichScreenType, int gearPartGain, int energyPointGain)
    {
        switch (whichScreenType)
        {
            case ScreenType.GameOver:
                gameOverScreenPanel.SetActive(true);
                enemyDefeatedScreenPanel.SetActive(false);
                break;
            case ScreenType.CollectLoot:
                gameOverScreenPanel.SetActive(false);
                enemyDefeatedScreenPanel.SetActive(true);
                gearPartsAmountText.text = gearPartGain.ToString();
                energyPointAmountText.text = energyPointGain.ToString();
                break;
        }
        gameObject.SetActive(true);
    }

    void CollectLoot()
    {
        gameObject.SetActive(false);
        CombatManager.GetInstance().ReturnToChamber();
    }

    void RestartGame()
    {
        gameObject.SetActive(false);
        CombatManager.GetInstance().RestartGame();
    }
}
