using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventChoice : MonoBehaviour
{
    [SerializeField] Button eventChoiceButton;
    [SerializeField] TextMeshProUGUI choiceDescription;
    private EventManagerUI eventManagerUIReference;
    private EventOutcome eventOutcome;
    
    public void SetEventChoice(EventOutcome eventOutcomeRef, EventManagerUI eventManagerUI)
    {
        eventOutcome = eventOutcomeRef;
        eventManagerUIReference = eventManagerUI;
        choiceDescription.text = eventOutcome.outcomeDesription;
        eventChoiceButton.onClick.AddListener(SelectChoice);
    }

    void SelectChoice()
    {
        eventManagerUIReference.SelectChoice(eventOutcome);
    }
}
