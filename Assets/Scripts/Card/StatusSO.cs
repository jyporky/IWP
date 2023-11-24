using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeywordSO", menuName = "ScriptableObject/Keyword")]
public class StatusSO : ScriptableObject
{
    [Header("StatusInfo")]
    public string statusName;
    [TextArea] public string statusDescription;
    public Sprite statusSprite;
    public bool ignoreKeyword;
}
