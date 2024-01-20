using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCard : CardBase
{
    [SerializeField] GameObject boughtOverlay;
    [SerializeField] float originalScale;
    [SerializeField] float expandedScale;

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
}
