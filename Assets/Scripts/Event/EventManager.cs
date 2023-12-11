using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EventOutcome
{
    [TextArea]
    public string outcomeDesription;
    [TextArea]
    public string outcomeChosenDialogue;
    public List<OutcomeEffect> listOfOutcomeEffects;
}

[System.Serializable]
public class OutcomeEffect
{
    public EventOutcomeType eventOutcomeType;
    public int amtChanged;
    public CardSO cardReferenceIfAny;
}

public enum EventOutcomeType
{
    Gain_Health,
    Gain_Health_Percentage,
    Gain_Energy,
    Gain_Max_HP,
    Gain_Max_SP,
    Gain_Gear_Parts,
    Add_Card,
    Upgrade_Random_Card,
}

public class EventManager : MonoBehaviour
{
    private static EventManager instance;
    public static EventManager GetInstance()
    {
        return instance;
    }

    private List<EventSO> eventOutcomesList = new List<EventSO>();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        UpdateEventOutcomeList();
    }

    /// <summary>
    /// Fetch the list of possible events from the chamberManager and update the list.
    /// </summary>
    void UpdateEventOutcomeList()
    {
        eventOutcomesList.Clear();

        List<EventSO> tempList = ChamberManager.GetInstance()?.GetPossibleEventsFromCurrentChamber();
        foreach (EventSO eventSO in tempList)
        {
            eventOutcomesList.Add(eventSO);
        }
    }

    /// <summary>
    /// Get a random event from the event List.
    /// </summary>
    /// <returns></returns>
    public EventSO GetRandomEvent()
    {
        int eventIndex = Random.Range(0, eventOutcomesList.Count);
        return eventOutcomesList[eventIndex];
    }
}
