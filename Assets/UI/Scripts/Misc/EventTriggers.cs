using UnityEngine;
using UnityEngine.Events;

public class EventTriggers : MonoBehaviour
{
    [SerializeField] bool eventOnStart = false;
    [SerializeField] bool eventOnEnable = false;
    [SerializeField] bool eventOnDisable = false;
    [SerializeField] bool eventOnTrigger = false;
    public UnityEvent unityEvent;

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
        unityEvent?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (eventOnTrigger)
            unityEvent?.Invoke();
    }
}
