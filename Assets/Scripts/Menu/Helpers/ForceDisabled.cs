using UnityEngine;

public class ForceDisabled : MonoBehaviour
{
    private void Awake()
    {
        foreach (var behaviour in GetComponentsInChildren<IForceDisabledElement>(true)) behaviour.Setup();
    }

    private void OnDestroy()
    {
        foreach (var behaviour in GetComponentsInChildren<IForceDisabledElement>(true)) behaviour.OnDestruction();
    }
}
