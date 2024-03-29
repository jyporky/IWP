using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardManager : MonoBehaviour
{
    [Header("Upgradable Cards References")]
    [SerializeField] GameObject upgradeCardPrefab;
    [SerializeField] Transform upgradeCardSpawnArea;
    [SerializeField] Button closeUpgradeCardPanel;
    private List<CardSO> upgradableCardsList = new List<CardSO>();

    [Header("Upgrading Card Info References")]
    [SerializeField] GameObject upgradingCardInfoPanel;
    [SerializeField] SelectedCard baseCardReference;
    [SerializeField] SelectedCard upgradedCardReference;
    [SerializeField] TextMeshProUGUI upgradeCostText;
    [SerializeField] Color32 sufficientAmt = Color.white;
    [SerializeField] Color32 insufficientAmt = Color.red;
    [SerializeField] Button upgradeCardButton;
    [SerializeField] Button cancelUpgradeButton;
    private UpgradeStationManagerUI upgradeStationManagerUI;

    private PlayerManager pm;
    private GameplayManager gm;
    private UpgradeStationManager usm;
    private int upgradeCostAmount;

    private void Start()
    {
        closeUpgradeCardPanel.onClick.AddListener(CloseUpgradeCardManager);
        cancelUpgradeButton.onClick.AddListener(CloseUpgradingCardInfo);
        upgradeCardButton.onClick.AddListener(UpgradeCard);
        CloseUpgradingCardInfo();
        pm = PlayerManager.GetInstance();
        gm = GameplayManager.GetInstance();
        usm = UpgradeStationManager.GetInstance();
        LoadUpgradeCardList();
    }

    /// <summary>
    /// Set the reference to the upgradeStationManagerUI if needed.
    /// </summary>
    public void SetPurchaseRef(UpgradeStationManagerUI usmRef)
    {
        upgradeStationManagerUI = usmRef;
    }

    /// <summary>
    /// Destroy this panel.
    /// </summary>
    void CloseUpgradeCardManager()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Clear the upgradable card list and fetch the new list.
    /// </summary>
    void LoadCardList()
    {
        upgradableCardsList.Clear();
        upgradableCardsList = usm.GetUpgradableCards(pm.GetCardList());
    }

    /// <summary>
    /// Create and display the list of cards that is upgradable. Destroy the old list of cards first if any.
    /// </summary>
    void LoadUpgradeCardList()
    {
        // Fetch the newly updated list of upgradable cards from the playerManager.
        LoadCardList();

        // Delete the old list of upgradeCard gameobject first.
        UpgradeCard[] oldUpgradeCardList = upgradeCardSpawnArea.GetComponentsInChildren<UpgradeCard>();
        for (int i = oldUpgradeCardList.Length - 1; i >= 0; i--)
        {
            Destroy(oldUpgradeCardList[i].gameObject);
        }

        // Create the new list of upgradeCard gameobject.
        for (int i = 0; i < upgradableCardsList.Count; i++)
        {
            GameObject newUpgradableCard = Instantiate(upgradeCardPrefab, upgradeCardSpawnArea);
            newUpgradableCard.GetComponent<UpgradeCard>().UpdateCardDetails(upgradableCardsList[i], this);
        }
        CloseUpgradingCardInfo();
    }

    /// <summary>
    /// Select this card and preview the upgrade information.
    /// </summary>
    public void SelectCard(CardSO selectedCard)
    {
        // Update the card display
        UpgradeCardSO upgradeCardSO = usm.GetUpgradeCardSO(selectedCard);
        baseCardReference.UpdateCardDetails(upgradeCardSO.baseCard);
        upgradedCardReference.UpdateCardDetails(upgradeCardSO.upgradedCard);

        // Update the price as well as the button interactable
        upgradeCostAmount = upgradeCardSO.upgradeCost;
        upgradeCostText.text = upgradeCostAmount.ToString();
        int playerAmt = pm.GetGearPartsAmount();
        if (upgradeCardSO.upgradeCost <= playerAmt)
        {
            upgradeCostText.color = sufficientAmt;
            upgradeCardButton.interactable = true;
        }
        else
        {
            upgradeCostText.color = insufficientAmt;
            upgradeCardButton.interactable = false;
        }
        upgradingCardInfoPanel.SetActive(true);
    }

    /// <summary>
    /// CLose the panel that displays the upgrade info of the card.
    /// </summary>
    void CloseUpgradingCardInfo()
    {
        upgradingCardInfoPanel.SetActive(false);
    }

    /// <summary>
    /// Upgrade the selected card by removing it from the player deck and adding the upgraded version.
    /// </summary>
    void UpgradeCard()
    {
        pm.RemoveFromCardList(baseCardReference.GetCardSO());
        pm.ChangeCurrentGearAmount(-upgradeCostAmount);
        pm.AddToCardList(upgradedCardReference.GetCardSO());
        gm.UpdatePlayerStatsDisplay();
        LoadUpgradeCardList();
        upgradeStationManagerUI?.onPurchase?.Invoke();
    }
}
