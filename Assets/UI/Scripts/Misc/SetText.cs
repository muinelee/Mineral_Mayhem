using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Use functions from UnityEvent
/// </summary>
public class SetText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string originalText;

    //------------------------------//

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    private void Start()
    {
        originalText = text.text;
    }

    public void ChangeText(string newText)
    {
        text.text = newText;
    }

    public void RevertText()
    {
        text.text = originalText;
    }
}
