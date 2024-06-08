using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class DebugHide : MonoBehaviour
{
    [SerializeField] private GameObject debugPanel;

    private void Update()
    {
        CheckForHKeyPress();
    }

    private void CheckForHKeyPress()
    {
        if (!debugPanel) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            debugPanel.SetActive(!debugPanel.activeInHierarchy);
        }
    }
}
