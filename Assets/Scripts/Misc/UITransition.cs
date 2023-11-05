using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITransition : MonoBehaviour
{
    private static UITransition instance;
    public static UITransition GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
        transitionUIDisplayReference.SetActive(false);
        transitionBG = transitionUIDisplayReference.GetComponent<Image>();
    }

    public delegate void OnFinishTransition();
    public OnFinishTransition onFinishTransition;

    [SerializeField] private GameObject transitionUIDisplayReference;
    [SerializeField] private float fadeInAndOutTimer = 1.0f;
    private Image transitionBG;

    /// <summary>
    /// Start the transition effect of the fadein and out. Will invoke delegate event after the fadeIn.
    /// </summary>
    public void BeginTransition()
    {
        transitionBG.fillAmount = 0;
        transitionUIDisplayReference.SetActive(true);
        StartCoroutine(FadeInOutTransition());
    }

    /// <summary>
    /// Ienumerator for the fadeInOut animation. Will invoke the delegate event after fadeIn.
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeInOutTransition()
    {
        bool doneTransition = false;
        bool fadeIn = false;
        bool fadeOut = false;
        float fadeMaxTimer = fadeInAndOutTimer / 2;
        float fadeTimer = 0;

        while(!doneTransition)
        {
            if (!fadeIn)
            {
                fadeTimer += Time.deltaTime;
                transitionBG.fillAmount = Mathf.Lerp(0, 1, fadeTimer / fadeMaxTimer);

                if (fadeTimer >= fadeMaxTimer)
                {
                    onFinishTransition?.Invoke();
                    fadeIn = true;
                }
            }

            else if (!fadeOut)
            {
                fadeTimer -= Time.deltaTime;
                transitionBG.fillAmount = Mathf.Lerp(0, 1, fadeTimer / fadeMaxTimer);

                if (fadeTimer <= 0)
                {
                    fadeOut = true;
                }
            }

            else
            {
                doneTransition = true;
            }

            yield return null;
        }
        transitionUIDisplayReference.SetActive(false);
    }
}
