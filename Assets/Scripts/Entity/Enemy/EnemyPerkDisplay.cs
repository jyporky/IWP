using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EnemyPerkDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image perkImage;
    [SerializeField] TextMeshProUGUI perkDescriptionText;
    [SerializeField] GameObject perkBoxReference;

    /// <summary>
    /// Update the perk display within this script.
    /// </summary>
    public void UpdatePerkDisplay(Sprite perkSprite, string perkDescription)
    {
        perkImage.sprite = perkSprite;
        perkDescriptionText.text = perkDescription;
        perkBoxReference.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        perkBoxReference.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        perkBoxReference.SetActive(false);
    }
}
