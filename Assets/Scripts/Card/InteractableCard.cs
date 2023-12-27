using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractableCard : CardBase, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool selected = false;
    private bool playable = false;

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
            CombatManager.GetInstance().SelectCard(card, GetComponent<RectTransform>());
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
        if (playable)
            CombatManager.GetInstance().StartDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playable)
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

    /// <summary>
    /// Execute the parent, afterwards if player lacks the amount to play the card, do not allow the player to sart dragging the card.
    /// </summary>
    /// <param name="entityNexusAmt"></param>
    public override void UpdatePlayableState(int entityNexusAmt)
    {
        base.UpdatePlayableState(entityNexusAmt);

        // Set the card to a playable state or not depending on the amount.
        if (entityNexusAmt >= card.cardCost)
        {
            playable = true;
        }
        else
        {
            playable = false;
        }
    }
}
