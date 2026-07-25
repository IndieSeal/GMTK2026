using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : Singleton<TransitionManager>
{
    public static event Action TransitionStarted;
    public static event Action TransitionEnded;
    
    [SerializeField] private Image fadeImage;
    
    public IEnumerator FadeCoroutine(float fadeTime, Action onHide, Action onComplete)
    {
        TransitionStarted?.Invoke();
        
        Color targetColor = Color.black;
        Color transparentColor = new Color(0f, 0f, 0f, 0f);
        
        float maxValue = fadeTime / 2;
        
        float value = 0;
        while (value < maxValue)
        {
            value += Time.deltaTime;
            fadeImage.color = Color.Lerp(transparentColor, targetColor, value / maxValue);
            yield return null;
        }
        
        yield return null;

        fadeImage.color = targetColor;
        onHide?.Invoke();
        
        yield return null;

        value = 0;
        while (value < maxValue)
        {
            value += Time.deltaTime;
            fadeImage.color = Color.Lerp(targetColor, transparentColor, value / maxValue);
            yield return null;
        }
        fadeImage.color = transparentColor;

        onComplete?.Invoke();
        TransitionEnded?.Invoke();
    }
}