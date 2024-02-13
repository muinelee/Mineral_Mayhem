using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BTN_Animation : MonoBehaviour
{
    [Header("Lerp Settings")]
    [SerializeField] private float lerpDuration = 0.25f;
    private float startValue = 1;
    private float endValue = 1.25f;
    [SerializeField] float sizeMultiplier = -0.25f;
    [SerializeField] Color colorNormal;
    [SerializeField] Color colorPressed;

    private float curScale;
    private float lastScale;

    //-----------------------------------//

    private void Start()
    {
        curScale = transform.localScale.x;
        lastScale = curScale;

        endValue = startValue + (startValue * sizeMultiplier);
    }

    public void MouseEnter()
    {
        gameObject.GetComponent<Image>().color = colorPressed;
    }
    public void MouseExit()
    {
        gameObject.GetComponent<Image>().color = colorNormal;
    }

    public void MouseDown()
    {
        StopAllCoroutines();
        StartCoroutine(iMouseDown());
    }
    private IEnumerator iMouseDown()
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

    public void MouseUp()
    {
        StopAllCoroutines();
        StartCoroutine(iMouseUp());
    }
    private IEnumerator iMouseUp()
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
