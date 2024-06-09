using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(100)]
public class EventTriggers : MonoBehaviour
{
    [SerializeField] bool eventOnStart = false;
    [SerializeField] bool eventOnEnable = false;
    [SerializeField] bool eventOnDisable = false;
    [SerializeField] bool eventOnTrigger = false;
    public UnityEvent unityEvent;

    [SerializeField] float eventDelay = 0;

    //---------------------------------//

    private void Start()
    {
        if (eventOnStart)
            unityEvent?.Invoke();
    }
    private void OnEnable()
    {
        if (eventOnEnable)
            unityEvent?.Invoke();
    }
    private void OnDisable()
    {
        if (eventOnDisable)
            unityEvent?.Invoke();
    }

    public void CueEvent()
    {
        StartCoroutine(iCueEvent());
    }
    private IEnumerator iCueEvent()
    {
        yield return new WaitForSecondsRealtime(eventDelay);
        unityEvent?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (eventOnTrigger)
            unityEvent?.Invoke();
    }
}
