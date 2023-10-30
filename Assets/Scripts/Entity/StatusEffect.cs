using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StatusEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image statusImage;
    [SerializeField] TextMeshProUGUI statusDescription;
    [SerializeField] GameObject textBoxDescription;
    private Keyword statusEffectInfo;
    private Entity entityReference;
    private string baseDescription;
    private int duration;

    /// <summary>
    /// Set up the status effect
    /// </summary>
    public void SetStatus(Entity entity, Keyword statusInfo)
    {
        textBoxDescription.SetActive(false);
        // Assign the references
        entityReference = entity;
        statusEffectInfo = statusInfo;

        // assign the duration value acoording to whether it is delay or not
        switch (statusEffectInfo.keywordSO.delay)
        {
            case true:
                duration = statusEffectInfo.delayDuration;
                break;
            case false:
                duration = statusEffectInfo.duration;
                break;
        }

        // Update the display
        statusImage.sprite = statusEffectInfo.keywordSO.statusSprite;
        baseDescription = statusEffectInfo.keywordSO.statusName + ":" + statusEffectInfo.keywordSO.statusDescription;

        SetBaseDescription();

        // according to whether the duration is by card play or by turn, it will be assigned to the delegate event accordingly
        if (statusEffectInfo.keywordSO.durationByTurn)
        {
            entityReference.onEntityStartTurn += DecreaseDuration;
        }

        else
        {
            entityReference.onEntityPlayCard += DecreaseDuration;
            entityReference.onEntityEndTurn += EndTurn;
        }
    }

    /// <summary>
    /// Update the description accordingly.
    /// </summary>
    void UpdateText()
    {
        string extraText = "";

        switch (statusEffectInfo.keywordSO.durationByTurn)
        {
            case true:
                extraText = "(" + duration.ToString() + " turns left)";
                break;
            case false:
                extraText = "(" + duration.ToString() + " times left)";
                break;
        }

        statusDescription.text = baseDescription + extraText;
    }

    /// <summary>
    /// Set the base description to fetch from. This is given that the baseDescription contains something already.
    /// Only call this function at the start of the function
    /// </summary>
    void SetBaseDescription()
    {
        for (int i = 0; i < baseDescription.Length; i++)
        {
            if (baseDescription[i] == 'n')
            {
                if (i + 1 != baseDescription.Length && baseDescription[i + 1] == ' ')
                {
                    string front = baseDescription.Substring(0, i);
                    string value = statusEffectInfo.value.ToString();
                    string back = baseDescription.Substring(i + 1, baseDescription.Length - (i + 1));

                    baseDescription = front + value + back;
                    break;
                }
            }
        }
        UpdateText();
    }

    /// <summary>
    /// Get the value of the status.
    /// </summary>
    public int GetValue()
    {
        return statusEffectInfo.value;
    }

    /// <summary>
    /// Check to see if there is a delay for this status.
    /// </summary>
    public bool IsDelay()
    {
        return statusEffectInfo.keywordSO.delay;
    }

    /// <summary>
    /// Return true if the duration is by turn. Return false if the duration is by cards played
    /// </summary>
    public bool IsDurationByTurn()
    {
        return statusEffectInfo.keywordSO.durationByTurn;
    }

    /// <summary>
    /// Decrease the duration of this status effect. If it hits 0, it will delete itself and the dictonary of that keyword in the Entity class.
    /// If there is a delay, execute the card effect.
    /// </summary>
    void DecreaseDuration()
    {
        duration--;

        if (duration <= 0)
        {
            entityReference.onEntityStartTurn -= DecreaseDuration;
            entityReference.onEntityPlayCard -= DecreaseDuration;
            entityReference.onEntityEndTurn -= EndTurn;
            Destroy(gameObject);
            entityReference.RemoveStatusEffect(statusEffectInfo.keywordSO.keyword);
        }

        if (statusEffectInfo.keywordSO.delay)
        {
            GameplayManager.GetInstance().ExecuteCardFromDelay(entityReference, statusEffectInfo);
        }

        UpdateText();
    }

    /// <summary>
    /// If there is any card play duration for the effect. Remove the effect immediately without triggering its effect
    /// </summary>
    void EndTurn()
    {
        entityReference.onEntityPlayCard -= DecreaseDuration;
        entityReference.onEntityEndTurn -= EndTurn;
        entityReference.RemoveStatusEffect(statusEffectInfo.keywordSO.keyword);
        Destroy(gameObject);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        textBoxDescription.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textBoxDescription.SetActive(false);
    }
}
