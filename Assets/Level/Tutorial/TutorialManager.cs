using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class TutorialManager : MonoBehaviour
{
    //reference to the UI elements
    [Header("UI Elements")]
    [SerializeField] private CG_Fade tutorialConfirmUI;
    [SerializeField] private CG_Fade tutorialScreenUI;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private CG_Fade btnPrev;

    [Header("Tutorial Text")]
    [SerializeField] private string[] tutorialTexts;
    [SerializeField] private int index = 0;

    public void Deny() {
        //close the tutorial UI if they deny
        tutorialConfirmUI.gameObject.SetActive(true);
        tutorialConfirmUI.FadeOut();
    }

    public void StartTutorial() {
        //open the tutorial UI
        tutorialConfirmUI.gameObject.SetActive(true);
        tutorialConfirmUI.FadeIn();
    }

    //button ont the UI to start the tutorial
    public void ContinueTutorial() {
        //pressing confirm closes the confirm UI and opens the tutorial screen
        tutorialConfirmUI.gameObject.SetActive(true);
        tutorialConfirmUI.FadeOut();
        tutorialScreenUI.gameObject.SetActive(true);
        tutorialScreenUI.FadeIn();

        //calls the cycle index function
        index = 0;
        TutorialText(tutorialTexts[0]);
    }

    public void EndTutorial() {
        tutorialScreenUI.gameObject.SetActive(true);
        tutorialScreenUI.FadeOut();
    }

    public void TutorialText(string text) {
        tutorialText.text = text;
        //start coroutine to clear notifier
        StartCoroutine(ChangeText(10));
    }

    private IEnumerator ChangeText(int clearTime) {
        //wait for 10 seconds
        yield return new WaitForSeconds(clearTime);
        CycleIndex(true);
    }

    private void CycleIndex(bool next) 
    {
        if (next) index++;
        else index--;

        if (index == 0)
        {
            btnPrev.gameObject.SetActive(true);
            btnPrev.FadeOut();
        }
        else if (index == 1 && next == true)
        {
            btnPrev.gameObject.SetActive(true);
            btnPrev.FadeIn();
        }

        //if the index is greater than the length of the tutorial text array
        if (index >= tutorialTexts.Length)
        {
            //end the tutorial
            EndTutorial();
        }

        //send current index to the tutorial text
        TutorialText(tutorialTexts[index]);
    }

    public void NextHint() {
        StopAllCoroutines();
        CycleIndex(true);
    }

    public void PreviousHint() {
        StopAllCoroutines();
        CycleIndex(false);
    }
}
