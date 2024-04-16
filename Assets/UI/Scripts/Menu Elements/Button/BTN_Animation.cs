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
    private Color colorNormal;
    [SerializeField] Color colorPressed;
    [SerializeField] Image[] imagesToChangeColor;
    [SerializeField] TextMeshProUGUI[] textToChangeColor;

    private float curScale;
    private float lastScale;

    //-----------------------------------//

    private void Start()
    {
        startValue = transform.localScale.x;
        curScale = startValue;
        lastScale = curScale;

        endValue = startValue + (startValue * sizeMultiplier);

        colorNormal = gameObject.GetComponent<Image>().color;
    }

    public void ColorChange()
    {
        gameObject.GetComponent<Image>().color = colorPressed;

        if (imagesToChangeColor.Length > 0)
        {
            for (int i = 0; i < imagesToChangeColor.Length; i++)
            {
                imagesToChangeColor[i].color = colorPressed;
            }
        }
        if (textToChangeColor.Length > 0)
        {
            for (int i = 0; i < textToChangeColor.Length; i++)
            {
                textToChangeColor[i].color = colorPressed;
            }
        }
    }
    public void ColorRevert()
    {
        gameObject.GetComponent<Image>().color = colorNormal;

        if (imagesToChangeColor.Length > 0)
        {
            for (int i = 0; i < imagesToChangeColor.Length; i++)
            {
                imagesToChangeColor[i].color = colorNormal;
            }
        }
        if (textToChangeColor.Length > 0)
        {
            for (int i = 0; i < textToChangeColor.Length; i++)
            {
                textToChangeColor[i].color = colorNormal;
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
}
