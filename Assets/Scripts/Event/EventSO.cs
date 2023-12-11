using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSO", menuName = "ScriptableObject/EventInfo")]
public class EventSO : ScriptableObject
{
    public string eventName;
    [TextArea]
    public string eventDescription;
    public Sprite eventImage;
    public List<EventOutcome> listOfOutcome;
}
