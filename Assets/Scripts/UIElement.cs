using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour
{
    [SerializeField] protected CanvasRenderer canvasR;
    [SerializeField] protected float minAlpha;
    [SerializeField] protected float maxAlpha;
    [SerializeField] public float durationOfFade;
    [SerializeField] public Image image;

    public CanvasRenderer CanvasR => canvasR;
    
    IEnumerator LerpFade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            canvasR.SetAlpha(Mathf.Lerp(startAlpha, endAlpha, time / duration));
            time += Time.deltaTime;
            yield return null;
        }
        canvasR.SetAlpha(endAlpha);
    }

    public void Fade()
    {
        StartCoroutine(LerpFade(canvasR.GetAlpha(), minAlpha, durationOfFade));
    }
    
    public void Unfade()
    {
        StartCoroutine(LerpFade(canvasR.GetAlpha(), maxAlpha, durationOfFade));
    }

    public void ChangeImage (Sprite sprite)
    {
        image.sprite = sprite;
    }

    public virtual void Hide()
    { 
        canvasR.SetAlpha(minAlpha);
    }

    public virtual void Show()
    {
        canvasR.SetAlpha(maxAlpha);
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
