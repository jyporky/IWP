using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Floor", menuName = "ScriptableObject/Floor")]
public class FloorSO : ScriptableObject
{
    public string floorName;
    public string floorDescription;
}
