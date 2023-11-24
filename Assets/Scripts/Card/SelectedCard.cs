using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectedCard : CardBase
{
    [Header("Keyword Display Reference")]
    [SerializeField] GameObject keywordPrefab;
    [SerializeField] Transform keywordSpawnArea;

    public override void UpdateCardDetails(CardSO cardInfo)
    {
        base.UpdateCardDetails(cardInfo);
        DisplayKeywordList();
    }

    /// <summary>
    /// Display the list of effect(keywords) of the card if needed.
    /// Will ignore if it is part of the ignoreStatusList.
    /// </summary>
    void DisplayKeywordList()
    {
        KeywordDisplay[] keywordDisplayList = keywordSpawnArea.GetComponentsInChildren<KeywordDisplay>();
        for (int i = keywordDisplayList.Length - 1; i >= 0; i--)
        {
            Destroy(keywordDisplayList[i].gameObject);
        }

        List<Keyword> listOfStatus = card.keywordsList;
        for (int i = 0; i < listOfStatus.Count; i++)
        {
            if (listOfStatus[i] == null)
                continue;
            else if (listOfStatus[i].statusSO.ignoreKeyword)
                continue;
            else
            {
                GameObject newStatus = Instantiate(keywordPrefab, keywordSpawnArea);
                newStatus.GetComponent<KeywordDisplay>().UpdateKeywordDisplay(listOfStatus[i].statusSO.statusName, listOfStatus[i].statusSO.statusDescription);
            }
        }
    }
}
