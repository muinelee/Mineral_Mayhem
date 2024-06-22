using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOverlays : MonoBehaviour
{
    [SerializeField] CG_Fade overlayStorm1;
    [SerializeField] CG_Fade overlayStorm2;
    [SerializeField] CG_Fade overlayStorm3;

    [SerializeField] CG_Fade overlayHealth;

    [SerializeField] float delay = 2f;

    //-----------------------------------//

    public void ShowHealthOverlay(float healthPercent)
    {
        float alpha = ((healthPercent - 1f) * -1f) - 0.5f;
        overlayHealth.GetComponent<CanvasGroup>().alpha = alpha;
    }

    [ContextMenu("Exit Eye of Storm")]
    public void OnExitStorm()
    {
        StartCoroutine(iOnExitStorm());
    }
    private IEnumerator iOnExitStorm()
    {
        overlayStorm1.gameObject.SetActive(true);
        overlayStorm1.FadeIn();

        yield return new WaitForSeconds(delay);

        overlayStorm2.gameObject.SetActive(true);
        overlayStorm2.FadeIn();
        yield return new WaitForSeconds(delay);

        overlayStorm3.gameObject.SetActive(true);
        overlayStorm3.FadeIn();
    }

    [ContextMenu("Enter Eye of Storm")]
    public void OnEnterStorm()
    {
        StopAllCoroutines();
        overlayStorm1.gameObject.SetActive(true);
        overlayStorm1.FadeOut();
        overlayStorm2.gameObject.SetActive(true);
        overlayStorm2.FadeOut();
        overlayStorm3.gameObject.SetActive(true);
        overlayStorm3.FadeOut();
    }
}
