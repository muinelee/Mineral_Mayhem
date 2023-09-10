using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmountKilled : MonoBehaviour
{
    public TextMeshProUGUI amountKilledText;
    public int Killed;

    //display the amount of enemies killed when hitting game over
    void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            Killed = gameManager.amountKilled;
            Debug.Log("I did it");
            Debug.Log("Amount killed is " + Killed);
        }
        else
        {
            Debug.Log("I failed");
        }
        amountKilledText.text = "You killed " + Killed + " enemies!";
    }

}
