using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeywordSO", menuName = "ScriptableObject/Keyword")]
public class KeywordSO : ScriptableObject
{
    [Header("StatusInfo")]
    public string statusName;
    [TextArea] public string statusDescription;
    public Sprite statusSprite;

    [Header("KeywordInfo")]
    public KeywordType keyword;

    /// <summary>
    /// If there is a duration, true means it is by turn, false means it is by card played.
    /// </summary>
    public bool durationByTurn;

    /// <summary>
    /// If set to true, wait for the delayDuration to be over before executing the effect.
    /// </summary>
    public bool delay;
}
