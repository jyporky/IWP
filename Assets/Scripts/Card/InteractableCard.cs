using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableCard : CardBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool selected = false;

    private void Start()
    {
        CombatManager.GetInstance().onDragging += SetSelected;
        CombatManager.GetInstance().onGameEnd += Unsubscribe;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
        {
            GetComponent<CanvasGroup>().alpha = 0;
            CombatManager.GetInstance().SelectCard(card, transform);
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
        CombatManager.GetInstance().StartDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CombatManager.GetInstance().EndDrag();
        Deselected();
    }

    private void Deselected()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        CombatManager.GetInstance().DeselectCard();
    }

    void SetSelected(bool isSelected)
    {
        selected = isSelected;
    }

    /// <summary>
    /// Unsubscribe all delegate event that it is subscribed to
    /// </summary>
    void Unsubscribe()
    {
        CombatManager.GetInstance().onDragging -= SetSelected;
        CombatManager.GetInstance().onGameEnd -= Unsubscribe;
    }
}
