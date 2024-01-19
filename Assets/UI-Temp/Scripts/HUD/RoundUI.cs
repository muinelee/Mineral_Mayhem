using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundUI : MonoBehaviour
{
    int blueWins = 0;
    int redWins = 0;

    [Header("UI Element References")]
    [SerializeField] CG_Fade gemBlue1;
    [SerializeField] CG_Fade gemBlue2;
    [SerializeField] CG_Fade gemBlue3;
    [SerializeField] CG_Fade gemRed1;
    [SerializeField] CG_Fade gemRed2;
    [SerializeField] CG_Fade gemRed3;

    //------------------------------//

    private void Start()
    {
        gemBlue1.gameObject.SetActive(false);
        gemBlue2.gameObject.SetActive(false);
        gemBlue3.gameObject.SetActive(false);
        gemRed1.gameObject.SetActive(false);
        gemRed2.gameObject.SetActive(false);
        gemRed3.gameObject.SetActive(false);
    }

    public void UpdateRoundUI()
    {
        if (blueWins == 1)
        {
            gemBlue1.gameObject.SetActive(true);
            gemBlue1.FadeIn();
        }
        else if (blueWins == 2)
        {
            gemBlue2.gameObject.SetActive(true);
            gemBlue2.FadeIn();
        }
        else if (blueWins >= 3)
        {
            gemBlue3.gameObject.SetActive(true);
            gemBlue3.FadeIn();
        }

        if (redWins == 1)
        {
            gemRed1.gameObject.SetActive(true);
            gemRed1.FadeIn();
        }
        else if (redWins == 2)
        {
            gemRed2.gameObject.SetActive(true);
            gemRed2.FadeIn();
        }
        else if (redWins >= 3)
        {
            gemRed3.gameObject.SetActive(true);
            gemRed3.FadeIn();
        }
    }

    public void BlueWin()
    {
        if (blueWins == 0)
        {
            blueWins = 1;
            UpdateRoundUI();
        }
        else if (blueWins == 1)
        {
            blueWins = 2;
            UpdateRoundUI();
        }
        else if (blueWins >= 2)
        {
            blueWins = 3;
            UpdateRoundUI();
            GameOver(true);
        }
    }
    public void RedWin()
    {
        if (redWins == 0)
        {
            redWins = 1;
            UpdateRoundUI();
        }
        else if (redWins == 1)
        {
            redWins = 2;
            UpdateRoundUI();
        }
        else if (redWins >= 2)
        {
            redWins = 3;
            UpdateRoundUI();
            GameOver(false);
        }
    }

    private void GameOver(bool blue)
    {
        if (blue == true)
        { 
            
        }
        else
        {

        }
    }
}
