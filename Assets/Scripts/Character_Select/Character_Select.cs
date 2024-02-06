using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Select : MonoBehaviour {
    public GameManager gameManager;

    //[Header("Text")]
    //public Text characterDescription;  could be used or removed
    //public Text backstory;

    [Header("Buttons")]
    //reference the Pyre button
    public Button pyreButton;
    //reference the Crystra button
    public Button crystraButton;
    //reference the Terran button
    public Button terranButton;
    //reference the Luna button
    public Button lunaButton;

    [Header("Prefabs")]
    //reference Pyre prefab
    public GameObject PyrePrefab;
    //reference Crystra prefab
    public GameObject CrystraPrefab;
    //reference Terran prefab
    public GameObject TerranPrefab;
    //reference Luna prefab
    public GameObject LunaPrefab;

    [Header("Spawn Point")]
    //reference the spawn point
    public Transform spawnPoint;

    void Start() {
        Debug.Log("Character Select Loaded");
        if (pyreButton) {
            pyreButton.onClick.AddListener(PyreButton);
        }

        if (crystraButton) {
            crystraButton.onClick.AddListener(CrystraButton);
        }

        if (terranButton) {
            terranButton.onClick.AddListener(TerranButton);
        }

        if (lunaButton) {
            lunaButton.onClick.AddListener(LunaButton);
        }

    }

    //when clicking the Pyre button, call this function
    private void PyreButton() {
        Debug.Log("Pyre Button Clicked");
        //if another prefab present, destroy them
        if (GameObject.Find("Network_Player(Clone)")) {
            Destroy(GameObject.Find("Network_Player(Clone)"));
        }
        //spawn the prefab at the position of the spawn point
        Instantiate(PyrePrefab, spawnPoint.position, spawnPoint.rotation);

        //gameManager.UpdateSelectedClass("Pyre"); leaving as a hook for possible use
    }

    private void CrystraButton() {
        //if another prefab present, destroy them
        if (GameObject.Find("Network_Player(Clone)")) {
            Destroy(GameObject.Find("Network_Player(Clone)"));
        }
        Debug.Log("Crystra Button Clicked");
        //spawn the crystra prefab at the position of the spawn point
        Instantiate(CrystraPrefab, spawnPoint.position, spawnPoint.rotation);

        //gameManager.UpdateSelectedClass("Crystra"); leaving as a hook for possible use
    }

    private void TerranButton() {
        //if another prefab present, destroy them
        if (GameObject.Find("Network_Player(Clone)")) {
            Destroy(GameObject.Find("Network_Player(Clone)"));
        }
        Debug.Log("Terran Button Clicked");
        //spawn the terran prefab at the position of the spawn point
        Instantiate(TerranPrefab, spawnPoint.position, spawnPoint.rotation);

        //gameManager.UpdateSelectedClass("Terran"); leaving as a hook for possible use
    }

    private void LunaButton() {
        //if another prefab present, destroy them
        if (GameObject.Find("Network_Player(Clone)")) {
            Destroy(GameObject.Find("Network_Player(Clone)"));
        }
        Debug.Log("Luna Button Clicked");
        //spawn the luna prefab at the position of the spawn point
        Instantiate(LunaPrefab, spawnPoint.position, spawnPoint.rotation);

        //gameManager.UpdateSelectedClass("Luna"); leaving as a hook for possible use
    }
}