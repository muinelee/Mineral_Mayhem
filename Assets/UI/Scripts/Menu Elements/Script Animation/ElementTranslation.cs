using System.Collections;
using UnityEngine;

public class ElementTranslation : MonoBehaviour
{
    private RectTransform rectTR;

    [Header("Lerp Settings")]
    [SerializeField] private float lerpDuration = 0.25f;
    [SerializeField] Vector3 startPos = Vector3.zero;
    [SerializeField] Vector3 endPos = Vector3.zero;
    private Vector3 curPos;
    private Vector3 lastPos;

    //-----------------------------------------//

    private void Awake()
    {
        rectTR = GetComponent<RectTransform>();
    }

    public void TranslateChange(float delay)
    {
        StopAllCoroutines();
        StartCoroutine(iTranslateChange(delay));
    }
    private IEnumerator iTranslateChange(float delay)
    {
        yield return new WaitForSeconds(delay);

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curPos = Vector3.Lerp(lastPos, endPos, timeElapsed / lerpDuration);
            rectTR.anchoredPosition = curPos;
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        curPos = endPos;
        rectTR.anchoredPosition = curPos;
    }

    public void TranslateRevert(float delay)
    {
        StopAllCoroutines();
        StartCoroutine(iTranslateRevert(delay));
    }
    private IEnumerator iTranslateRevert(float delay)
    {
        yield return new WaitForSeconds(delay);

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curPos = Vector3.Lerp(lastPos, startPos, timeElapsed / lerpDuration);
            rectTR.anchoredPosition = curPos;
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        curPos = endPos;
        rectTR.anchoredPosition = curPos;
    }

    
}
