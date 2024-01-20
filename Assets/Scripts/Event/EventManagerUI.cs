using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManagerUI : MonoBehaviour
{
    [Header("Event Information Display")]
    [SerializeField] Image eventImageDisplay;
    [SerializeField] TextMeshProUGUI eventNameText;
    [SerializeField] TextMeshProUGUI eventDescriptionText;

    [Header("Event Choices references")]
    [SerializeField] GameObject eventChoicePrefab;
    [SerializeField] Transform eventChoicesSpawnArea;

    [Header("Misc")]
    [SerializeField] Button continueButton;

    private EventSO currentEvent;
    private EventManager eventManager;

    private void Start()
    {
        eventManager = EventManager.GetInstance();
        continueButton.onClick.AddListener(ExitEvent);
    }

    /// <summary>
    /// When loading into the event panel, call this function to reset the event.
    /// </summary>
    public void LoadToEvent()
    {
        continueButton.gameObject.SetActive(false);
        LoadEvent();
    }

    /// <summary>
    /// Get a random event from the eventmanager and display it. Create the options for the player to choose.
    /// </summary>
    void LoadEvent()
    {
        currentEvent = eventManager.GetRandomEvent();
        eventImageDisplay.sprite = currentEvent.eventImage;
        eventNameText.text = currentEvent.eventName;
        eventDescriptionText.text = currentEvent.eventDescription;

        List<EventOutcome> listOfPossibleOutcome = currentEvent.listOfOutcome;

        for (int i = 0; i < listOfPossibleOutcome.Count; i++)
        {
            GameObject newEventOutcome = Instantiate(eventChoicePrefab, eventChoicesSpawnArea);
            newEventOutcome.GetComponent<EventChoice>().SetEventChoice(listOfPossibleOutcome[i], this, CheckIfCanChoose(listOfPossibleOutcome[i]));
        }
    }

    /// <summary>
    /// Execute the choice according to what event choices is selected. <br/>
    /// Clear out the options and update the display.
    /// </summary>
    /// <param name="eventOutcome"></param>
    public void SelectChoice(EventOutcome eventOutcome)
    {
        // Clear out the options
        EventChoice[] listOfChoices = eventChoicesSpawnArea.GetComponentsInChildren<EventChoice>();
        for (int i = listOfChoices.Length - 1; i >= 0; i--)
        {
            Destroy(listOfChoices[i].gameObject);
        }
        // Execute the outcome effect
        ExecuteChoiceOutcomeEffect(eventOutcome);

        // Update the display
        eventDescriptionText.text = eventOutcome.outcomeChosenDialogue;
        continueButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Execute the effect of the outcomeType depending on the eventoutcome
    /// </summary>
    void ExecuteChoiceOutcomeEffect(EventOutcome eventOutcome)
    {
        PlayerManager pm = PlayerManager.GetInstance();
        for (int i = 0; i < eventOutcome.listOfOutcomeEffects.Count; i++)
        {
            OutcomeEffect eo = eventOutcome.listOfOutcomeEffects[i];
            switch (eo.eventOutcomeType)
            {
                case EventOutcomeType.Gain_Health:
                    pm.SetCurrentHealth(pm.GetCurrentHealth() + eo.amtChanged);
                    break;
                case EventOutcomeType.Gain_Health_Percentage:
                    float changeAmountBy = pm.GetMaxHP() * (eo.amtChanged / 100.0f);
                    pm.SetCurrentHealth(pm.GetCurrentHealth() + (int)changeAmountBy);
                    break;
                case EventOutcomeType.Gain_Energy:
                    pm.SetCurrentEnergyPoint(pm.GetCurrentEP() + eo.amtChanged);
                    break;
                case EventOutcomeType.Gain_Max_HP:
                    pm.ModifyIncreaseHP(eo.amtChanged);
                    break;
                case EventOutcomeType.Gain_Max_SP:
                    pm.ModifyIncreaseEP(eo.amtChanged);
                    break;
                case EventOutcomeType.Gain_Gear_Parts:
                    pm.ChangeCurrentGearAmount(eo.amtChanged);
                    break;
                case EventOutcomeType.Add_Card:
                    if (eo.amtChanged > 0)
                    {
                        for (int amt = 0; amt < eo.amtChanged; amt++)
                        {
                            pm.AddToCardList(eo.cardReferenceIfAny);
                        }
                    }
                    else if (eo.amtChanged < 0)
                    {
                        for (int amt = 0; amt < -eo.amtChanged; amt++)
                        {
                            pm.RemoveFromCardList(eo.cardReferenceIfAny);
                        }
                    }
                    break;
                case EventOutcomeType.Delete_Card:
                    EventManager.GetInstance().OpenDeleteCardPanel(eo.amtChanged);
                    break;
            }
        }
        GameplayManager.GetInstance().UpdatePlayerStatsDisplay();
    }

    /// <summary>
    /// Check to see if the player fulfill the requirements to choose the options if a penalty is involved.
    /// </summary>
    /// <returns></returns>
    bool CheckIfCanChoose(EventOutcome eventOutcome)
    {
        PlayerManager pm = PlayerManager.GetInstance();
        foreach (OutcomeEffect oe in eventOutcome.listOfOutcomeEffects)
        {
            if (oe.amtChanged < 0 || oe.eventOutcomeType == EventOutcomeType.Delete_Card)
            {
                switch (oe.eventOutcomeType)
                {
                    case EventOutcomeType.Gain_Health:
                        if (-oe.amtChanged >= pm.GetCurrentHealth())
                            return false;

                        break;
                    case EventOutcomeType.Gain_Health_Percentage:
                        int changeAmountBy = pm.GetMaxHP() * (int)(oe.amtChanged / 100.0f);
                        if (-changeAmountBy >= pm.GetCurrentHealth())
                            return false;
                        break;
                    case EventOutcomeType.Gain_Energy:
                        if (-oe.amtChanged > pm.GetCurrentEP())
                            return false;

                        break;
                    case EventOutcomeType.Gain_Max_HP:
                        if (-oe.amtChanged >= pm.GetMaxHP())
                            return false;

                        break;
                    case EventOutcomeType.Gain_Max_SP:
                        if (-oe.amtChanged > pm.GetMaxEP())
                            return false;

                        break;
                    case EventOutcomeType.Gain_Gear_Parts:
                        if (-oe.amtChanged > pm.GetGearPartsAmount())
                            return false;

                        break;
                    case EventOutcomeType.Add_Card:
                        if (-oe.amtChanged > pm.GetCardCount(oe.cardReferenceIfAny))
                            return false;

                        break;
                    case EventOutcomeType.Delete_Card:
                        if (pm.GetCardList().Count == 0)
                            return false;

                        break;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Exit the event panel.
    /// </summary>
    void ExitEvent()
    {
        UITransition.GetInstance().BeginTransition(result =>
        {
            ChamberManager.GetInstance().ClearRoom();
        });
    }
}
