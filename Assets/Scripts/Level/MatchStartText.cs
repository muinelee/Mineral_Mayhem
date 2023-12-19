using UnityEngine;
using TMPro;

[RequireComponent(typeof(CanvasGroup))]
public class MatchStartText : MonoBehaviour
{
    public float displayDuration = 2f;
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // Start with the text invisible
        canvasGroup.alpha = 0f;

        // For testing purposes, call DisplayMatchText immediately
        Invoke("DisplayMatchText", 0f);
    }

    // Call this method to trigger the display of the text
    public void DisplayMatchStartText()
    {
        // Invoke DisplayMatchText after a delay
        Invoke("DisplayMatchText", 0f);
    }

    private void DisplayMatchText()
    {
        Debug.Log("Fading in...");

        // Fade in
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime / displayDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
        }

        // Wait for the specified duration
        Debug.Log("Waiting...");
        Invoke("FadeOut", displayDuration);
    }

    private void FadeOut()
    {
        Debug.Log("Fading out...");

        // Fade out
        float timer = 0f;
        while (timer < 1f)
        {
            timer += Time.deltaTime / displayDuration;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
        }

        Debug.Log("Fading complete");
    }
}

