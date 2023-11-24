using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : CardBase, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject keywordPrefab;
    [SerializeField] Transform keywordSpawnArea;
    [SerializeField] GameObject boughtOverlay;
    [SerializeField] float originalScale;
    [SerializeField] float expandedScale;

    public void OnPointerEnter(PointerEventData eventData)
    {
        DisplayKeywordList();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyKeywordList();
    }

    public override void UpdateCardDetails(CardSO cardInfo)
    {
        base.UpdateCardDetails(cardInfo);
        boughtOverlay.SetActive(false);
    }

    /// <summary>
    /// Enable the overlay of the boughtOverlay
    /// </summary>
    public void Bought()
    {
        boughtOverlay.SetActive(true);
    }

    /// <summary>
    /// Display the list of effect(keywords) of the card if needed.
    /// Will ignore if it is part of the ignoreStatusList.
    /// </summary>
    void DisplayKeywordList()
    {
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

    void DestroyKeywordList()
    {
        KeywordDisplay[] keywordDisplayList = keywordSpawnArea.GetComponentsInChildren<KeywordDisplay>();
        for (int i = keywordDisplayList.Length - 1; i >= 0; i--)
        {
            Destroy(keywordDisplayList[i].gameObject);
        }
    }
}
