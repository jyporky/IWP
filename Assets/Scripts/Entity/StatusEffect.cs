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
    private StatusEffectInfo statusEffectInfo;
    private KeywordType whichStatus;
    private Entity entityReference;
    private string baseDescription;

    /// <summary>
    /// Set up the status effect
    /// </summary>
    public void SetStatus(Entity entity, KeywordType keywordType, StatusEffectInfo statusInfo)
    {
        textBoxDescription.SetActive(false);
        entityReference = entity;
        whichStatus = keywordType;
        statusEffectInfo = statusInfo;
        StatusSpriteInfo tempSpriteInfo = CardManager.GetInstance().GetStatusSpriteInfo(statusEffectInfo, whichStatus);

        if (tempSpriteInfo != null)
        {
            statusImage.sprite = tempSpriteInfo.statusSprite;
            baseDescription = tempSpriteInfo.statusName + ":" + tempSpriteInfo.statusDescription;
        }
        SetBaseDescription();

        if (statusEffectInfo.delay)
        {
            if (statusEffectInfo.delay && statusEffectInfo.delayDuration > 0)
                entityReference.onEntityStartTurn += DecreaseTurn;

            else if (statusEffectInfo.delay && statusEffectInfo.delayDuration < 0)
            {
                entityReference.onEntityPlayCard += DecreaseCardPlayed;
                entityReference.onEntityEndTurn += EndTurn;
            }
        }

        else
        {
            if (statusEffectInfo.duration > 0)
                entityReference.onEntityStartTurn += DecreaseTurn;

            else if (statusEffectInfo.duration < 0)
            {
                entityReference.onEntityPlayCard += DecreaseCardPlayed;
                entityReference.onEntityEndTurn += EndTurn;
            }
        }
    }

    /// <summary>
    /// Update the description accordingly.
    /// </summary>
    void UpdateText()
    {
        string extraText = "";

        if (!statusEffectInfo.delay)
        {
            if (statusEffectInfo.duration > 0)
                extraText = "(" + statusEffectInfo.duration.ToString() + " turns left)";

            else if (statusEffectInfo.duration < 0)
                extraText = "(" + (-statusEffectInfo.duration).ToString() + " times left)";
        }

        else
        {
            if (statusEffectInfo.delayDuration > 0)
                extraText = "(" + statusEffectInfo.delayDuration.ToString() + " turns left)";

            else if (statusEffectInfo.delayDuration < 0)
                extraText = "(" + (-statusEffectInfo.delayDuration).ToString() + " times left)";
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
        return statusEffectInfo.delay;
    }

    /// <summary>
    /// Get the duration of the status.
    /// </summary>
    public int GetDuration()
    {
        return statusEffectInfo.duration;
    }

    /// <summary>
    /// Decrease the number of turns in the duration of the keyword. If the duration reaches 0, the effect varies.
    /// Delay duration hits 0: activate card effect. Duration hits 0: Destroy this status effect.
    /// </summary>
    void DecreaseTurn()
    {
        switch (statusEffectInfo.delay)
        {
            case true:
                {
                    statusEffectInfo.delayDuration--;

                    if (statusEffectInfo.delayDuration <= 0)
                    {
                        entityReference.onEntityStartTurn -= DecreaseTurn;
                        Destroy(gameObject);
                        entityReference.RemoveStatusEffect(whichStatus);
                        CardManager.GetInstance().ExecuteCardFromDelay(entityReference, statusEffectInfo, whichStatus);
                    }
                }
                break;

            case false:
                {
                    statusEffectInfo.duration--;

                    if (statusEffectInfo.duration <= 0)
                    {
                        entityReference.onEntityStartTurn -= DecreaseTurn;
                        Destroy(gameObject);
                        entityReference.RemoveStatusEffect(whichStatus);
                    }
                }
                break;
        }
        UpdateText();
    }

    /// <summary>
    /// Decrease the amount of card play duration for the effect. If the card effect requires 3 cards, playing 1 reducing it to 2.
    /// </summary>
    void DecreaseCardPlayed()
    {
        switch (statusEffectInfo.delay)
        {
            case true:
                {
                    statusEffectInfo.delayDuration++;

                    if (statusEffectInfo.delayDuration >= 0)
                    {
                        EndTurn();
                        CardManager.GetInstance().ExecuteCardFromDelay(entityReference, statusEffectInfo, whichStatus);
                    }
                }
                break;

            case false:
                {
                    statusEffectInfo.duration++;

                    if (statusEffectInfo.duration >= 0)
                    {
                        EndTurn();
                    }
                }
                break;
        }
        UpdateText();
    }

    /// <summary>
    /// If there is any card play duration for the effect. Remove the effect immediately without triggering its effect
    /// </summary>
    void EndTurn()
    {
        entityReference.onEntityPlayCard -= DecreaseCardPlayed;
        entityReference.onEntityEndTurn -= EndTurn;
        entityReference.RemoveStatusEffect(whichStatus);
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
