using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class NetworkPlayer_InGameUI : NetworkBehaviour
{   
    [SerializeField] private SO_NetworkAttack qAttack;
    [SerializeField] private SO_NetworkAttack eAttack;
    [SerializeField] private SO_NetworkDash dash;

    private NetworkPlayer_Attack playerAttack;
    private NetworkPlayer_Movement playerMovement;
    private NetworkPlayer_Health playerHealth;

    [Header("Health bar")]
    [SerializeField] private Slider healthBar;

    // cooldown stuff
    [Header("Dash Properties")]
    [SerializeField] private Image dashIcon;
    [SerializeField] private Image dashImageCooldown;
    [SerializeField] private Text dashCooldownText;

    [Header("Q Attack Properties")]
    [SerializeField] private Image qAttackIcon;
    [SerializeField] private Image qImageCooldown;
    [SerializeField] private Text qCooldownText;

    [Header("E Attack Properties")]
    [SerializeField] private Image eAttackIcon;
    [SerializeField] private Image eImageCooldown;
    [SerializeField] private Text eCooldownText;

    // cooldown comes from Scriptable Objects passed from local player

    public override void FixedUpdateNetwork()
    {
        // Update HealthBar
        DisplayHealth();

        // Update dash UI display
        DisplayAbilityCooldown(ref playerMovement.GetDashCoolDownTimer(), dashImageCooldown, dashCooldownText, dash.GetCoolDown());

        // Update Q Spell UI display
        DisplayAbilityCooldown(ref playerAttack.GetQAttackCoolDownTimer(), qImageCooldown, qCooldownText, qAttack.GetCoolDown());
        
        // Update E Spell UI display
        DisplayAbilityCooldown(ref playerAttack.GetEAttackCoolDownTimer(), eImageCooldown, eCooldownText, eAttack.GetCoolDown());
    }

    private void DisplayAbilityCooldown(ref TickTimer coolDownTimer, Image coolDownImage, Text coolDownText, float maxCoolDown)
    {
        //display the cooldown
        if (!coolDownTimer.IsRunning)
        {
            coolDownText.text = "";
            coolDownImage.fillAmount = 0;
        }
        else
        {
            coolDownText.text = ((float)coolDownTimer.RemainingTime(Runner)).ToString("F2");
            coolDownImage.fillAmount = (float)coolDownTimer.RemainingTime(Runner) / maxCoolDown;
        }
    }

    private void DisplayHealth()
    {
        if (healthBar.value != playerHealth.HP / playerHealth.GetStartingHP() && healthBar.value > 0) healthBar.value = playerHealth.HP / playerHealth.GetStartingHP();
    }

    public void PrimeUI()
    {
        dashIcon.sprite = dash.GetDashIcon();
        qAttackIcon.sprite = qAttack.GetAttackIcon();
        eAttackIcon.sprite = eAttack.GetAttackIcon();
    }

    public void SetDash(SO_NetworkDash newDash)
    {
        dash = newDash;
    }

    public void SetQAttack(SO_NetworkAttack newAttack)
    {
        qAttack = newAttack;
    }

    public void SetEAttack(SO_NetworkAttack newAttack)
    {
        eAttack = newAttack;
    }

    public void SetPlayerAttack(NetworkPlayer_Attack playerAttackScript)
    {
        playerAttack = playerAttackScript;
    }

    public void SetPlayerMovement(NetworkPlayer_Movement playerMovementScript)
    {
        playerMovement = playerMovementScript;
    }

    public void SetPlayerHealth(NetworkPlayer_Health playerHealthScript)
    {
        playerHealth = playerHealthScript;
    }
}