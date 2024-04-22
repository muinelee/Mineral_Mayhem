using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RotateObject : MonoBehaviour
{
    [Header("Lerp Settings")]
    [SerializeField] private float lerpDuration = 0.25f;
    private Vector3 curRotation;

    [SerializeField] float rotateDelay = 0.5f;
    [SerializeField] float eventDelay = 1.5f;
    public UnityEvent OnRotateFinished;

    //-----------------------------------//

    public void Rotate(float degrees)
    {
        StopAllCoroutines();
        StartCoroutine(iRotate(degrees));
    }
    private IEnumerator iRotate(float degrees)
    {
        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 endRotation = new Vector3(0, degrees, 0);

        yield return new WaitForSecondsRealtime(rotateDelay);

        float timeElapsed = 0;
        while (timeElapsed < lerpDuration)
        {

            curRotation = new Vector3(0, Mathf.Lerp(startRotation.y, endRotation.y, timeElapsed / lerpDuration), 0);
            transform.localEulerAngles = curRotation;
            timeElapsed += Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.005f);
        }
        transform.eulerAngles = endRotation;

        yield return new WaitForSecondsRealtime(eventDelay);

        OnRotateFinished?.Invoke();
    }
}
