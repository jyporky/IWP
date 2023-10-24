using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] Image cardAttackTypeSprite;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] Image cardSprite;
    [SerializeField] TextMeshProUGUI cardDescription;
    private CardSO card;
    private bool selected = false;

    private void Start()
    {
        GameplayManager.GetInstance().onDragging += SetSelected;
    }

    public void UpdateCardDetails(CardSO cardInfo)
    {
        card = cardInfo;
        cardName.text = cardInfo.cardName;
        cardSprite.sprite = cardInfo.cardSprite;
        cardDescription.text = cardInfo.cardDescription;
    }

    public CardSO GetCardSO()
    {
        return card;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GameplayManager.GetInstance().SelectCard(card, transform);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            Deselected();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameplayManager.GetInstance().StartDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameplayManager.GetInstance().EndDrag();
        Deselected();
    }

    private void Deselected()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GameplayManager.GetInstance().DeselectCard();
    }

    void SetSelected(bool isSelected)
    {
        selected = isSelected;
    }
}
