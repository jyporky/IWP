using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeleteCardManager : MonoBehaviour
{
    [Header("Upgradable Cards References")]
    [SerializeField] TextMeshProUGUI amountText;
    [SerializeField] GameObject deleteCardPrefab;
    [SerializeField] Transform deleteCardSpawnArea;
    private List<CardSO> upgradableCardsList = new List<CardSO>();

    [Header("Upgrading Card Info References")]
    [SerializeField] GameObject deletingCardInfoPanel;
    [SerializeField] SelectedCard deletedCardReference;
    [SerializeField] Button deleteCardButton;
    [SerializeField] Button cancelDeleteButton;

    private PlayerManager pm;
    private GameplayManager gm;
    [SerializeField] private int counter;

    private void Start()
    {
        cancelDeleteButton.onClick.AddListener(CloseUpgradingCardInfo);
        deleteCardButton.onClick.AddListener(DeleteCard);
        CloseUpgradingCardInfo();
        pm = PlayerManager.GetInstance();
        gm = GameplayManager.GetInstance();
        LoadDeleteCardList();
    }

    /// <summary>
    /// Set the amount of cards player can delete before the panel destroy itself.
    /// </summary>
    public void SetCounter(int counterAmt)
    {
        counter = counterAmt;
    }

    /// <summary>
    /// Destroy this panel.
    /// </summary>
    void CloseDeleteCardManager()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Clear the deletable card list and fetch the new list.
    /// </summary>
    void LoadCardList()
    {
        upgradableCardsList.Clear();
        upgradableCardsList = new List<CardSO>(pm.GetCardList());
    }

    /// <summary>
    /// Create and display the list of cards that is deletable. Destroy the old list of cards first if any.
    /// </summary>
    void LoadDeleteCardList()
    {
        // Fetch the newly updated list of upgradable cards from the playerManager.
        LoadCardList();

        // Delete the old list of upgradeCard gameobject first.
        DeleteCard[] oldUpgradeCardList = deleteCardSpawnArea.GetComponentsInChildren<DeleteCard>();
        for (int i = oldUpgradeCardList.Length - 1; i >= 0; i--)
        {
            Destroy(oldUpgradeCardList[i].gameObject);
        }

        // Create the new list of upgradeCard gameobject.
        for (int i = 0; i < upgradableCardsList.Count; i++)
        {
            GameObject newUpgradableCard = Instantiate(deleteCardPrefab, deleteCardSpawnArea);
            newUpgradableCard.GetComponent<DeleteCard>().UpdateCardDetails(upgradableCardsList[i], this);
        }

        // show how many times player needs to delete their cards
        amountText.text = "(" + counter.ToString() + " times)";

        CloseUpgradingCardInfo();
    }

    /// <summary>
    /// Select this card and display more information about the card.
    /// </summary>
    public void SelectCard(CardSO selectedCard)
    {
        // Update the card display
        deletedCardReference.UpdateCardDetails(selectedCard);
        deletingCardInfoPanel.SetActive(true);
    }

    /// <summary>
    /// CLose the panel that displays the upgrade info of the card.
    /// </summary>
    void CloseUpgradingCardInfo()
    {
        deletingCardInfoPanel.SetActive(false);
    }

    /// <summary>
    /// Upgrade the selected card by removing it from the player deck and adding the upgraded version.
    /// </summary>
    void DeleteCard()
    {
        counter--;
        pm.RemoveFromCardList(deletedCardReference.GetCardSO());
        LoadDeleteCardList();
        gm.UpdatePlayerStatsDisplay();

        if (counter <= 0)
        {
            CloseDeleteCardManager();
        }
    }
}
