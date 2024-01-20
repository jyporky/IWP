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
    Delete_Card,
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

        UISpawnArea = GameObject.FindGameObjectWithTag("GameplayUISpawn").transform;
    }

    [SerializeField] GameObject deleteCardPrefab;
    private Transform UISpawnArea;

    /// <summary>
    /// Add to the list of possible events that can be encountered.
    /// </summary>
    public void AddToEventList(List<EventSO> eventList)
    {
        eventOutcomesList = new(eventList);
    }

    /// <summary>
    /// Clear the list of possible events that can be encountered.
    /// </summary>
    public void ClearEventList()
    {
        eventOutcomesList.Clear();
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

    /// <summary>
    /// Open up the delete card panel. Input the amount of cards to be deleted.
    /// </summary>
    public void OpenDeleteCardPanel(int cardsToDelete = 1)
    {
        DeleteCardManager deleteCardManager = Instantiate(deleteCardPrefab, UISpawnArea).GetComponent<DeleteCardManager>();
        deleteCardManager.SetCounter(cardsToDelete);
    }
}
