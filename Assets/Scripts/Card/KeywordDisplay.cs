using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeywordDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI keywordName;
    [SerializeField] TextMeshProUGUI keywordDescription;

    /// <summary>
    /// Update the name and the description of the keyword
    /// </summary>
    public void UpdateKeywordDisplay(string name, string description)
    {
        keywordName.text = name;
        keywordDescription.text = description;
    }
}
