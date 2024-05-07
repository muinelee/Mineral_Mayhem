using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

public class TutorialManager : MonoBehaviour
{
    //reference to the UI elements
    [Header("UI Elements")]
    [SerializeField] private GameObject tutorialConfirmUI;
    [SerializeField] private GameObject tutorialUI;
    [SerializeField] private GameObject tutorialScreenUI;
    [SerializeField] private TMP_Text tutorialText;

    [Header("Tutorial Text")]
    [SerializeField] private string[] tutorialTexts;
    [SerializeField] private int index = 0;
    private bool tutorialTimer = false;

    public void Update() {
        CharacterSelect.OnCharacterSelect += StartTutorial;
    }
    public void Deny() {
        //close the tutorial UI if they deny
        tutorialUI.SetActive(false);
    }

    public void StartTutorial() {
        //open the tutorial UI
        tutorialUI.SetActive(true);
    }

    //button ont the UI to start the tutorial
    public void ContinueTutorial() {
        //pressing confirm closes the confirm UI and opens the tutorial screen
        tutorialConfirmUI.SetActive(false);
        tutorialScreenUI.SetActive(true);
        //sets timer to true
        tutorialTimer = true;
        //calls the cycle index function
        CycleIndex();
    }

    public void EndTutorial() {
        tutorialScreenUI.SetActive(false);
    }

    public void TutorialText(string text) {
        tutorialText.text = text;
        //start coroutine to clear notifier
        StartCoroutine(ChangeText(5));
    }

    private IEnumerator ChangeText(int clearTime) {
        //wait for 5 seconds
        yield return new WaitForSeconds(clearTime);
        tutorialTimer = true;
        CycleIndex();
    }

    private void CycleIndex() {
        //if the timer is true
        if (tutorialTimer) {
            //set the timer to false
            tutorialTimer = false;
            //send current index to the tutorial text
            TutorialText(tutorialTexts[index]);
            //increment the index
            index++;
            //if the index is greater than the length of the tutorial text array
            if (index >= tutorialTexts.Length) {
                //end the tutorial
                EndTutorial();
            }
        }
    }

    public void NextHint() {
        index++;
        if (index >= tutorialTexts.Length) {
            EndTutorial();
        }
        else {
            //stop coroutine
            StopAllCoroutines();
            TutorialText(tutorialTexts[index]);
        }
    }

    public void PreviousHint() {
        index--;
        if (index < 0) {
            index = 0;
        }
        else {
            //stop coroutine
            StopAllCoroutines();
            TutorialText(tutorialTexts[index]);
        }
    }
}
