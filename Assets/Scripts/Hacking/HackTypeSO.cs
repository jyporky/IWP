using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HackTypeSO", menuName = "ScriptableObject/HackType")]
public class HackTypeSO : ScriptableObject
{
    public HackType hackType;
    [TextArea]
    public string hackDescription;
    public int duration;
    public int amount;
    public List<CardType> combinationList = new List<CardType>();
}
