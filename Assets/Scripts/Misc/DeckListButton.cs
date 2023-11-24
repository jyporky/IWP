using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckListButton : MonoBehaviour
{
    [SerializeField] Button deckListButton;
    [SerializeField] GameObject deckListUIPrefab;
    private Transform gameplayUISpawnArea;

    private void Start()
    {
        gameplayUISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
        deckListButton.onClick.AddListener(OpenDeckListPanel);
    }

    /// <summary>
    /// Create the Deck List panel. Put in the player deck List as the parameter into the function. 
    /// </summary>
    void OpenDeckListPanel()
    {
        GameObject newDeckListPanel = Instantiate(deckListUIPrefab, gameplayUISpawnArea);
        newDeckListPanel.GetComponent<DeckList>().SetupDeckList(PlayerManager.GetInstance().GetCardList());
    }
}
