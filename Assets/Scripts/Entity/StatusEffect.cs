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
    private string modifiedDescription;
    private bool isStatusDelay;
    private int statusDuration;

    /// <summary>
    /// Set up the status effect
    /// </summary>
    public void SetStatus(Entity entity, Keyword statusInfo)
    {
        CombatManager.GetInstance().onGameEnd += Unsubscribe;
        textBoxDescription.SetActive(false);
        // Assign the references
        entityReference = entity;
        statusEffectInfo = new(statusInfo);

        // assign the duration value as well as the status information display according to whether it is delay or not
        switch (statusEffectInfo.cardDelay.statusInfo == null)
        {
            case true:
                {
                    StatusSO ss = statusEffectInfo.statusSO;
                    statusDuration = statusEffectInfo.duration;
                    isStatusDelay = false;
                    statusImage.sprite = ss.statusSprite;
                    baseDescription = ss.statusName + ":" + ss.statusDescription;
                    break;
                }

            case false:
                {
                    StatusSO ss = statusEffectInfo.cardDelay.statusInfo;
                    statusDuration = statusEffectInfo.cardDelay.duration;
                    isStatusDelay = true;
                    statusImage.sprite = ss.statusSprite;
                    baseDescription = ss.statusDescription;
                    break;
                }
        }

        SetModifiedDescription();

        // according to whether the duration is by card play or by turn, it will be assigned to the delegate event accordingly
        if ((isStatusDelay &&  statusEffectInfo.cardDelay.durationByTurn)|| statusEffectInfo.durationByTurn)
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

        if (!isStatusDelay)
        {
            switch (statusEffectInfo.durationByTurn)
            {
                case true:
                    extraText = "(" + statusDuration.ToString() + " turns left)";
                    break;
                case false:
                    extraText = "(" + statusDuration.ToString() + " times left)";
                    break;
            }
        }

        else
        {
            switch (statusEffectInfo.cardDelay.durationByTurn)
            {
                case true:
                    extraText = statusDuration.ToString() + " turns";
                    break;
                case false:
                    extraText = "playing " + statusDuration.ToString() + " more cards.";
                    break;
            }
        }

        statusDescription.text = modifiedDescription + extraText;
    }

    /// <summary>
    /// Decrease the value of the keyword by 1, should it hit 0, destroy this status.
    /// </summary>
    public void DecreaseValue()
    {
        statusEffectInfo.value--;

        if (statusEffectInfo.value <= 0)
        {
            Unsubscribe();
            Destroy(gameObject);
            entityReference.RemoveStatusEffect(statusEffectInfo.keywordType, gameObject);
        }
    }

    /// <summary>
    /// Set the modified description to fetch from. The modified description is based on the based description and how the value is changed.
    /// </summary>
    void SetModifiedDescription()
    {
        for (int i = 0; i < baseDescription.Length; i++)
        {
            if (baseDescription[i] == 'n')
            {
                if (i + 1 != baseDescription.Length && i != 0 && baseDescription[i + 1] == ' ' && baseDescription[i - 1] == ' ')
                {
                    string front = baseDescription.Substring(0, i);

                    string value;

                    switch (!isStatusDelay)
                    {
                        case true:
                            value = statusEffectInfo.value.ToString();
                            break;

                        case false:
                            value = statusEffectInfo.statusSO.statusName + "(" + statusEffectInfo.value + ") to ";
                            switch (statusEffectInfo.inflictSelf)
                            {
                                case true:
                                    value += "self";
                                    break;
                                case false:
                                    value += "opponent";
                                    break;
                            }

                            break;
                    }


                    string back = baseDescription.Substring(i + 1, baseDescription.Length - (i + 1));

                    modifiedDescription = front + value + back;
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
        return (isStatusDelay);
    }

    /// <summary>
    /// Return true if the duration is by turn. Return false if the duration is by cards played
    /// </summary>
    public bool IsDurationByTurn()
    {
        return statusEffectInfo.durationByTurn;
    }

    /// <summary>
    /// Get the duration of the status.
    /// </summary>
    /// <returns></returns>
    public int GetDuration()
    {
        return statusDuration;
    }

    /// <summary>
    /// Add the value to the existing value.
    /// </summary>
    public void UpdateValue(int changeBy)
    {
        statusEffectInfo.value += changeBy;
        SetModifiedDescription();
    }

    /// <summary>
    /// Decrease the duration of this status effect. If it hits 0, it will delete itself and the dictonary of that keyword in the Entity class.
    /// If there is a delay, execute the card effect.
    /// </summary>
    void DecreaseDuration()
    {
        statusDuration--;

        if (statusDuration <= 0)
        {
            Unsubscribe();
            Destroy(gameObject);
            entityReference.RemoveStatusEffect(statusEffectInfo.keywordType, gameObject);

            if (isStatusDelay)
            {
                Keyword newKeyword = new Keyword(statusEffectInfo, true);
                CombatManager.GetInstance().ExecuteCardFromDelay(entityReference, newKeyword);
            }
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
        entityReference.RemoveStatusEffect(statusEffectInfo.keywordType, gameObject);
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

    /// <summary>
    /// Unsubscribe from any delegate event it is subscribed to.
    /// </summary>
    void Unsubscribe()
    {
        CombatManager.GetInstance().onGameEnd -= Unsubscribe;
        entityReference.onEntityStartTurn -= DecreaseDuration;
        entityReference.onEntityPlayCard -= DecreaseDuration;
        entityReference.onEntityEndTurn -= EndTurn;
    }
}
