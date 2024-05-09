using System.Collections;
using UnityEngine;

public class CG_ScreenFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;

    [Header("OpacityLerp Settings")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float startValue = 0;
    [SerializeField] private float endValue = 1f;

    private float curValue;

    //--------------------------------------//

    private void Start()
    {
        FadeOut();
    }

    public void FadeIn()
    {
        group.gameObject.SetActive(true);
        StartCoroutine(iFadeIn());
    }
    private IEnumerator iFadeIn()
    {
        group.alpha = startValue;

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curValue = Mathf.Lerp(startValue, endValue, timeElapsed / lerpDuration);
            group.alpha = curValue;

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue = endValue;
        group.alpha = curValue;
    }

    public void FadeOut()
    {
        group.gameObject.SetActive(true);
        StartCoroutine(iFadeOut());
    }
    private IEnumerator iFadeOut()
    {
        group.alpha = endValue;
        Debug.Log("!");

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curValue = Mathf.Lerp(endValue, startValue, timeElapsed / lerpDuration);
            group.alpha = curValue;

            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        curValue = startValue;
        group.alpha = curValue;

        group.gameObject.SetActive(false);
    }
}
