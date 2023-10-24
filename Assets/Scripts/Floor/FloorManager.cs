using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FloorManager : MonoBehaviour
{
    [SerializeField] Button nextFloorButton;
    [SerializeField] Button previousFloorButton;
    [SerializeField] TextMeshProUGUI floorName;
    [SerializeField] TextMeshProUGUI floorDescription;
    private int currentFloorNumIndex;
    [SerializeField] List<FloorSO> floorList;

    private void Start()
    {
        currentFloorNumIndex = 0;
        UpdateFloor();
        nextFloorButton.onClick.AddListener(GoNextFloor);
        previousFloorButton.onClick.AddListener(GoPreviousFloor);
    }

    /// <summary>
    /// Go to the next floor in the list and update the UI.
    /// </summary>
    void GoNextFloor()
    {
        currentFloorNumIndex += 1;
        UpdateFloor();
    }

    /// <summary>
    /// Go to the previous floor in the list and update the UI.
    /// </summary>
    void GoPreviousFloor()
    {
        currentFloorNumIndex -= 1;
        UpdateFloor();
    }

    /// <summary>
    /// Update the display of the current floor, and disable any nextFloor/previousFloor buttons if needed.
    /// </summary>
    void UpdateFloor()
    {
        // Disable the floor buttons if needed
        if (currentFloorNumIndex - 1 < 0)
            previousFloorButton.interactable = false;
        else
            previousFloorButton.interactable = true;

        if (currentFloorNumIndex + 1 >= floorList.Count)
            nextFloorButton.interactable = false;
        else
            nextFloorButton.interactable = true;

        floorName.text = floorList[currentFloorNumIndex].floorName;
        floorDescription.text = floorList[currentFloorNumIndex].floorDescription;
    }
}
