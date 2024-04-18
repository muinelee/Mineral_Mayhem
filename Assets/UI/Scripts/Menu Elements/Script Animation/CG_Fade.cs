using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CG_Fade : MonoBehaviour
{
    private CanvasGroup group;

    [Header("OpacityLerp Settings")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float startValue = 0;
    [SerializeField] private float endValue = 1f;

    private float curValue;
    [SerializeField] float delayOpen;

    //--------------------------------------//

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        StartCoroutine(iFadeIn());
    }
    private IEnumerator iFadeIn()
    {
        group.alpha = startValue;
        yield return new WaitForSecondsRealtime(delayOpen);

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
        StartCoroutine(iFadeOut());
    }
    private IEnumerator iFadeOut()
    {
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

       gameObject.SetActive(false);
    }
}
