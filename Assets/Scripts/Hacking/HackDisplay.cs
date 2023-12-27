using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HackDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hack Display Info")]
    [SerializeField] Image hackIcon;
    [SerializeField] TextMeshProUGUI hackNameText;
    [SerializeField] TextMeshProUGUI hackDescriptionText;

    [Header("Hack Current Info")]
    [SerializeField] TextMeshProUGUI levelAmountDisplay;
    [SerializeField] TextMeshProUGUI turnsLeftAmountDisplay;

    [Header("Upgraded Description Info")]
    [SerializeField] GameObject upgradeHoverIcon;
    [SerializeField] GameObject UpgradedDescriptionReference;
    [SerializeField] TextMeshProUGUI UpgradedDescriptionText;

    [Header("Combination Info")]
    [SerializeField] GameObject combinationDisplayPrefab;
    [SerializeField] GameObject combinationListDisplayReference;
    [SerializeField] GameObject maxedUpgradeTextReference;

    [Header("Upgrade Button")]
    [SerializeField] Button self;

    // Stored Values
    private HackType hackType;
    private int hackLvl;
    private int hackMaxLevel;
    private int duration;

    private List<CardType> currentHackCombination = new List<CardType>();

    private HackingManager hm;
    private AssetManager am;
    private Player player;

    // Store the upgradable lvl boolean
    private bool upgradable = false;


    /// <summary>
    /// Setup the hacktype as well as the level of the hackDisplay script. <br/>
    /// set the player reference to this script.
    /// </summary>
    /// <param name="whichHackType"></param>
    public void SetHackType(HackType whichHackType, Player playerRef)
    {
        // Set the player instance into this script.
        player = playerRef;

        // Get instance of the hackingManager and assetManager
        hm = HackingManager.GetInstance();
        am = AssetManager.GetInstance();

        // Set up the base of the hackDisplay script. Set the value of the hackType, hackLvl, hackMaxLevel as well as the hack info name.
        hackType = whichHackType;
        hackLvl = 0;
        hackMaxLevel = PlayerManager.GetInstance().GetHackTypeMaxLevel(hackType);
        hackNameText.text = hm.GetHackTypeName(hackType);

        // Disable any tabs that is not suppose to be shown
        UpgradedDescriptionReference.SetActive(false);

        // Add button click listener to the upgrade hack lvl
        self.onClick.AddListener(UpgradeLvl);

        // Set the button inactive first
        self.interactable = false;

        UpdateHackDisplay();
    }

    /// <summary>
    /// Update the display when the level of the hackInfo changes.
    /// </summary>
    void UpdateHackDisplay()
    {
        // Get the next lvl number as well as display the current level.
        int nextLvl = hackLvl + 1;
        levelAmountDisplay.text = hackLvl.ToString();

        // Clear the current hack combination list as well as
        currentHackCombination.Clear();
        Image[] combinationList = combinationListDisplayReference.GetComponentsInChildren<Image>();

        for (int i = combinationList.Length - 1; i > 0; i--)
        {
            Destroy(combinationList[i].gameObject);
        }

        // Display the UI for the current level info. What is displayed depend on whether the current level is 0 or not.
        switch (hackLvl)
        {
            case 0:
                hackDescriptionText.text = "Inactive. Preview upgrade to see its effect.";
                turnsLeftAmountDisplay.text = "-";
                break;
            default:
                HackTypeSO currLvlRef = hm.GetHackTypeSO(hackType, hackLvl);
                hackDescriptionText.text = currLvlRef.hackDescription;
                duration = currLvlRef.duration;
                turnsLeftAmountDisplay.text = duration.ToString();
                break;
        }

        // Display the UI for the next level info. What is displayed depend on whether the next level exceeds the maximum level.
        if (nextLvl <= hackMaxLevel)
        {
            combinationListDisplayReference.SetActive(true);
            maxedUpgradeTextReference.SetActive(false);
            HackTypeSO currLvlRef = hm.GetHackTypeSO(hackType, nextLvl);
            foreach (var r in currLvlRef.combinationList)
            {
                GameObject newHack = Instantiate(combinationDisplayPrefab, combinationListDisplayReference.transform);
                newHack.GetComponent<Image>().sprite = am.GetCardSprite(r);
            }
            UpgradedDescriptionText.text = currLvlRef.hackDescription;
            upgradeHoverIcon.SetActive(true);

            foreach(var r in currLvlRef.combinationList)
            {
                currentHackCombination.Add(r);
            }

            upgradable = true;
        }
        else
        {
            combinationListDisplayReference.SetActive(false);
            maxedUpgradeTextReference.SetActive(true);
            upgradeHoverIcon.SetActive(false);

            upgradable = false;
            UpgradedDescriptionReference.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (upgradable)
            UpgradedDescriptionReference.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (upgradable)
            UpgradedDescriptionReference.SetActive(false);
    }

    /// <summary>
    /// Increase the current hack level by 1.
    /// </summary>
    void UpgradeLvl()
    {
        hackLvl++;
        player.HackExecuted(currentHackCombination);
        ToggleHackUpgrade(false);
        UpdateHackDisplay();
    }

    /// <summary>
    /// Check to see if the cardTypeList provided matches the hackInfo combination. <br/>
    /// As long as the order and type provided matches the combination, the upgrade will be avaliable. <br/>
    /// Works even if the provided cardTypeList contains more cardType than the script combination. <br/>
    /// If this hack has reach the max lvl. It will skip the check.
    /// </summary>
    /// <param name="cardTypeList"></param>
    public void AttemptHack(List<CardType> cardTypeList)
    {
        if (hackLvl == hackMaxLevel)
            return;

        if (currentHackCombination.Count > cardTypeList.Count)
        {
            ToggleHackUpgrade(false);
            return;
        }

        for (int i = 0; i < currentHackCombination.Count; i++)
        {
            if (cardTypeList[i] != currentHackCombination[i])
            {
                ToggleHackUpgrade(false);
                return;
            }
        }
        ToggleHackUpgrade(true);
    }

    /// <summary>
    /// Toggle whether the hack info can be upgraded or not.
    /// </summary>
    void ToggleHackUpgrade(bool canUpgrade)
    {
        self.interactable = canUpgrade;
    }

    /// <summary>
    /// Reduce the duration of the current hack level if possible.
    /// </summary>
    public void ReduceDuration()
    {
        // If the duration is 0, do not reduce.
        if (duration == 0)
            return;

        if (duration != 0)
        {
            duration--;
            turnsLeftAmountDisplay.text = duration.ToString();
        }

        if (duration == 0)
        {
            hackLvl--;
            ToggleHackUpgrade(false);
            UpdateHackDisplay();
        }
    }

    /// <summary>
    /// Get the current level of this hack info.
    /// </summary>
    /// <returns></returns>
    public int GetHackLvl()
    {
        return hackLvl;
    }

    /// <summary>
    /// Get the hackType of this hack info.
    /// </summary>
    /// <returns></returns>
    public HackType GetHackType()
    {
        return hackType;
    }
}
