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

    private NetworkPlayer_Attack playerAttack;
    private NetworkPlayer_Health playerHealth;

    [Header("Health bar")]
    [SerializeField] private Slider healthBar;

    // cooldown stuff
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

        // Update Q Spell UI display
        DisplaySpellCooldown(ref playerAttack.GetQAttackCoolDownTimer(), qImageCooldown, qCooldownText, qAttack.GetCoolDown());
        
        // Update E Spell UI display
        DisplaySpellCooldown(ref playerAttack.GetEAttackCoolDownTimer(), eImageCooldown, eCooldownText, eAttack.GetCoolDown());
    }

    private void DisplaySpellCooldown(ref TickTimer coolDownTimer, Image coolDownImage, Text coolDownText, float maxCoolDown)
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
        Debug.Log($"Health bar value is {healthBar.value}");
    }

    public void PrimeUI()
    {
        qAttackIcon.sprite = qAttack.GetAttackIcon();
        eAttackIcon.sprite = eAttack.GetAttackIcon();
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

    public void SetPlayerHealth(NetworkPlayer_Health playerHealthScript)
    {
        playerHealth = playerHealthScript;
    }
}