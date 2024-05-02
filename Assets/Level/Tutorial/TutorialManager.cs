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

    public void ContinueTutorial() {
        tutorialConfirmUI.SetActive(false);
        tutorialScreenUI.SetActive(true);
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
        tutorialText.text = "";
        CycleIndex();
    }

    private void CycleIndex() {
        //send current index to the tutorial text
        TutorialText(tutorialTexts[index]);
        //increment the index
        index++;
        if (index >= tutorialTexts.Length) {
            EndTutorial();
        }
    }
}
