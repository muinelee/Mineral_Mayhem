 using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BTN_Animation : MonoBehaviour
{
    [Header("Lerp Settings")]
    [SerializeField] private float lerpDuration = 0.25f;
    private float startValue = 1;
    private float endValue = 1.25f;
    [SerializeField] float sizeMultiplier = -0.25f;

    [Header("MouseOver/Off Animation")]
    [SerializeField] Image[] imagesToChangeColor;
    [SerializeField] Color[] colorNormal;
    [SerializeField] Color[] colorChanged;
    [SerializeField] Color[] colorDisabled;
    [SerializeField] Image[] imagesToSpriteSwap;
    [SerializeField] Sprite[] spriteNormal;
    [SerializeField] Sprite[] spriteChanged;
    [SerializeField] Sprite[] spriteDisabled;
    [SerializeField] TextMeshProUGUI[] textToChangeFont;
    [SerializeField] TMP_FontAsset[] fontNormal;
    [SerializeField] TMP_FontAsset[] fontChanged;
    [SerializeField] TMP_FontAsset[] fontDisabled;

    
    [SerializeField] bool lockColor;

    private float curScale;
    private float lastScale;

    //-----------------------------------//

    private void Start()
    {
        startValue = transform.localScale.x;
        curScale = startValue;
        lastScale = curScale;

        endValue = startValue + (startValue * sizeMultiplier);
    }
    private void OnDisable()
    {
        SpriteRevert();
    }

    public void SpriteChange()
    {
        if (!lockColor)
        {
            if (imagesToSpriteSwap.Length > 0)
            {
                for (int i = 0; i < imagesToSpriteSwap.Length; i++)
                {
                    imagesToSpriteSwap[i].sprite = spriteChanged[i];
                }
            }
            if (textToChangeFont.Length > 0)
            {
                for (int i = 0; i < textToChangeFont.Length; i++)
                {
                    textToChangeFont[i].font = fontChanged[i];
                }
            }
        }
    }
    public void SpriteRevert()
    {
        if (!lockColor)
        {
            if (imagesToSpriteSwap.Length > 0)
            {
                for (int i = 0; i < imagesToSpriteSwap.Length; i++)
                {
                    imagesToSpriteSwap[i].sprite = spriteNormal[i];
                }
            }
            if (textToChangeFont.Length > 0)
            {
                for (int i = 0; i < textToChangeFont.Length; i++)
                {
                    textToChangeFont[i].font = fontNormal[i];
                }
            }
        }
    }
    public void SpriteDisable()
    {
        if (imagesToSpriteSwap.Length > 0)
        {
            for (int i = 0; i < imagesToSpriteSwap.Length; i++)
            {
                imagesToSpriteSwap[i].sprite = spriteDisabled[i];
            }
        }
        if (textToChangeFont.Length > 0)
        {
            for (int i = 0; i < textToChangeFont.Length; i++)
            {
                textToChangeFont[i].font = fontDisabled[i];
            }
        }
    }

    public void ColorChange()
    {
        if (imagesToChangeColor.Length > 0)
        {
            for (int i = 0; i < imagesToChangeColor.Length; i++)
            {
                imagesToChangeColor[i].color = colorChanged[i];
            }
        }
    }
    public void ColorRevert()
    {
        if (imagesToChangeColor.Length > 0)
        {
            for (int i = 0; i < imagesToChangeColor.Length; i++)
            {
                imagesToChangeColor[i].color = colorNormal[i];
            }
        }
    }
    public void ColorDisable()
    {
        if (imagesToChangeColor.Length > 0)
        {
            for (int i = 0; i < imagesToChangeColor.Length; i++)
            {
                imagesToChangeColor[i].color = colorDisabled[i];
            }
        }
    }

    public void ScaleChange()
    {
        StopAllCoroutines();
        StartCoroutine(iScaleChange());
    }
    private IEnumerator iScaleChange()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curScale = Mathf.Lerp(lastScale, endValue, timeElapsed / lerpDuration);
            transform.localScale = new Vector3(curScale, curScale, curScale);
            lastScale = curScale;
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        curScale = endValue;
        transform.localScale = new Vector3(curScale, curScale, curScale);
    }

    public void ScaleRevert()
    {
        StopAllCoroutines();
        StartCoroutine(iScaleRevert());
    }
    private IEnumerator iScaleRevert()
    {
        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curScale = Mathf.Lerp(lastScale, startValue, timeElapsed / lerpDuration);
            transform.localScale = new Vector3(curScale, curScale, curScale);
            lastScale = curScale;
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        curScale = startValue;
        transform.localScale = new Vector3(curScale, curScale, curScale);
    }

    public void ColorLock(bool on)
    {
        lockColor = on;
    }
}
