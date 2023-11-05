using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomIconDisplay : MonoBehaviour
{
    private Room_Type roomType;
    private List<GameObject> pathList = new List<GameObject>();

    void Start()
    {
        ChamberManager.GetInstance().destroySelfAfterChamber += DestroySelf;
    }

    /// <summary>
    /// Update the sprite display according to the sprite input. Automatically register the DestroySelf function to the delegate event from ChamberManager.
    /// </summary>
    /// <param name="iconSprite"></param>
    public void UpdateSpriteIcon(Sprite iconSprite)
    {
        GetComponent<Image>().sprite = iconSprite;
    }

    /// <summary>
    /// Set the room type of this room gameobject.
    /// </summary>
    public void UpdateRoomType(Room_Type whatRoomType)
    {
        roomType = whatRoomType;
    }

    /// <summary>
    /// Destroy itself and unregister itself from the delegate event
    /// </summary>
    void DestroySelf()
    {
        ChamberManager.GetInstance().destroySelfAfterChamber -= DestroySelf;
        Destroy(gameObject);
    }

    /// <summary>
    /// Change the color of the image
    /// </summary>
    public void ChangeColor(Color32 colorToChangeTo)
    {
        GetComponent<Image>().color = colorToChangeTo;
    }

    /// <summary>
    /// Add the path to the list, this will be destroy or set not active accordingly.
    /// </summary>
    public void AddPathToList(GameObject pathReference)
    {
        pathList.Add(pathReference);
    }

    /// <summary>
    /// Set the path to active or inactive depending on the parameter
    /// </summary>
    public void SetPathActive(bool setActive)
    {
        for (int i = 0; i < pathList.Count; i++)
        {
            pathList[i].SetActive(setActive);
        }
    }

    /// <summary>
    /// Destroy all the paths in this roomIcon.
    /// </summary>
    public void DestroyPaths()
    {
        for (int i = pathList.Count - 1; i >= 0; i--)
        {
            Destroy(pathList[i]);
        }

        pathList.Clear();
    }
}
