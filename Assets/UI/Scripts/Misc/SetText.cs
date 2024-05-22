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
        StopAllCoroutines();
        StartCoroutine(iChangeText(newText));
    }
    private IEnumerator iChangeText(string newText)
    {
        yield return new WaitForSecondsRealtime(0.2f);

        text.text = newText;
    }

    public void RevertText()
    {
        StopAllCoroutines();
        StartCoroutine(iRevertText());
    }
    private IEnumerator iRevertText()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        text.text = originalText;
    }
}
