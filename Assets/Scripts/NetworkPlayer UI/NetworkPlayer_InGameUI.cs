using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class NetworkPlayer_InGameUI : MonoBehaviour
{
    public static NetworkPlayer_InGameUI instance { get; set; }

    // Abilities
    private SO_NetworkAttack qAttack;
    private SO_NetworkAttack eAttack;
    private SO_NetworkUlt fAttack;
    private SO_NetworkDash dash;

    // Components
    private NetworkPlayer_Health playerHealth;
    private NetworkPlayer_Energy playerEnergy;
    private NetworkPlayer_Movement playerMovement;
    private NetworkPlayer_Attack playerAttack;

    // Canvas Group Fades
    [SerializeField] private CG_Fade groupStats;
    [SerializeField] private CG_Fade groupAbilities;

    // UI Properties
    [Header("Health bar")]
    [SerializeField] private Slider healthBar;

    [Header("Energy bar")]
    [SerializeField] private Slider energyBar;

    // Character Icon
    [SerializeField] private Image charIcon;

    // Cooldown stuff
    [Header("Dash Properties")]
    [SerializeField] private Image dashBackground;
    [SerializeField] private Image dashIcon;
    [SerializeField] private Image dashImageCooldown;
    [SerializeField] private TextMeshProUGUI dashCooldownText;

    [Header("Q Attack Properties")]
    [SerializeField] private Image qAttackBackground;
    [SerializeField] private Image qAttackIcon;
    [SerializeField] private Image qImageCooldown;
    [SerializeField] private TextMeshProUGUI qCooldownText;

    [Header("E Attack Properties")]
    [SerializeField] private Image eAttackBackground;
    [SerializeField] private Image eAttackIcon;
    [SerializeField] private Image eImageCooldown;
    [SerializeField] private TextMeshProUGUI eCooldownText;

    [Header("F (ULT) Attack Properties")]
    [SerializeField] private Image fAttackBackground;
    [SerializeField] private Image fAttackIcon;
    [SerializeField] private Image fImageCooldown;
    [SerializeField] private Image fBorderFlair;

    [Header("Asset References")]
    [SerializeField] private Sprite iconCrysta;
    [SerializeField] private Sprite iconLuna;
    [SerializeField] private Sprite iconPyre;
    [SerializeField] private Sprite iconTerran;

    [SerializeField] private Sprite borderFlairCrysta;
    [SerializeField] private Sprite borderFlairPyre;

    private bool uIDisplayed = false;
    // cooldown comes from Scriptable Objects passed from local player
    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        if (!uIDisplayed) return;

        // Update Health Bar
        DisplayHealth();

        // Update Energy Bar
        DisplayEnergy();

        // Update dash UI display
        DisplayAbilityCooldown(playerMovement.GetDashCoolDownTimer(), dashImageCooldown, dashCooldownText, dash.GetCoolDown());

        // Update Q Spell UI display
        DisplayAbilityCooldown(playerAttack.GetQAttackCoolDownTimer(), qImageCooldown, qCooldownText, qAttack.GetCoolDown());
        
        // Update E Spell UI display
        DisplayAbilityCooldown(playerAttack.GetEAttackCoolDownTimer(), eImageCooldown, eCooldownText, eAttack.GetCoolDown());

        // Update F Spell UI display
        DisplayAbilityCooldown(fImageCooldown, playerEnergy.GetEnergyPercentage());
    }

    public void PrimeUI()
    {
        dashIcon.sprite = dash.GetDashIcon();
        dashBackground.sprite = dash.GetDashBackground();

        qAttackIcon.sprite = qAttack.GetAttackIcon();
        qAttackBackground.sprite = qAttack.GetAttackBackground();

        eAttackIcon.sprite = eAttack.GetAttackIcon();
        eAttackBackground.sprite = eAttack.GetAttackBackground();

        fAttackIcon.sprite = fAttack.GetAttackIcon();
        fAttackBackground.sprite = fAttack.GetAttackBackground();

        if (NetworkPlayer.Local.CharacterID == 0)
        {
            charIcon.sprite = iconCrysta;
            fBorderFlair.sprite = borderFlairCrysta;
        }
        else if (NetworkPlayer.Local.CharacterID == 1)
        {
            charIcon.sprite = iconLuna;
        }
        else if (NetworkPlayer.Local.CharacterID == 2)
        {
            charIcon.sprite = iconPyre;
            fBorderFlair.sprite = borderFlairPyre;
        }
        else if (NetworkPlayer.Local.CharacterID == 3)
        {
            charIcon.sprite = iconTerran;
        }
    }

    private void DisplayHealth()
    {
        if (healthBar.value <= 0) healthBar.value = 0;

        healthBar.value = playerHealth.HP / playerHealth.GetStartingHP();
    }

    private void DisplayEnergy()
    {
        energyBar.value = playerEnergy.GetEnergyPercentage();
    }

    private void DisplayAbilityCooldown(float coolDown, Image coolDownImage, TextMeshProUGUI coolDownText, float maxCoolDown)
    {
        //display the cooldown
        if (coolDown == 0)
        {
            coolDownText.text = "";
            coolDownImage.fillAmount = 1;
        }
        else
        {
            coolDownText.text = coolDown.ToString("0.0");
            coolDownImage.fillAmount = 1 - (coolDown / maxCoolDown);
        }

        if (coolDown == 0 && coolDownImage == dashImageCooldown)
        {
            dashBackground.sprite = dash.GetDashBackground();
        }
        else if (coolDownImage == dashImageCooldown)
        {
            dashBackground.sprite = dash.GetDashBackgroundGrey();
        }

        if (coolDown == 0 && coolDownImage == qImageCooldown)
        {
            qAttackBackground.sprite = qAttack.GetAttackBackground();
        }
        else if (coolDownImage == qImageCooldown)
        {
            qAttackBackground.sprite = qAttack.GetAttackBackgroundGrey();
        }

        if (coolDown == 0 && coolDownImage == eImageCooldown)
        {
            eAttackBackground.sprite = eAttack.GetAttackBackground();
        }
        else if (coolDownImage == eImageCooldown)
        {
            eAttackBackground.sprite = eAttack.GetAttackBackgroundGrey();
        }
    }

    private void DisplayAbilityCooldown(Image coolDownImage, float energyFill)
    {
        coolDownImage.fillAmount = energyFill;

        if (coolDownImage.fillAmount >= 1)
        {
            fAttackBackground.sprite = fAttack.GetAttackBackground();
        }
        else
        {
            fAttackBackground.sprite = fAttack.GetAttackBackgroundGrey();
        }
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

    public void SetFAttack(SO_NetworkUlt newUlt)
    {
        fAttack = newUlt;
    }

    public void SetPlayerHealth(NetworkPlayer_Health playerHealthScript)
    {
        playerHealth = playerHealthScript;
    }

    public void SetPlayerEnergy(NetworkPlayer_Energy playerEnergyScript)
    {
        playerEnergy = playerEnergyScript;
    }

    public void SetPlayerMovement(NetworkPlayer_Movement playerMovementScript)
    {
        playerMovement = playerMovementScript;
    }
    public void SetPlayerAttack(NetworkPlayer_Attack playerAttackScript)
    {
        playerAttack = playerAttackScript;
    }

    public void ShowPlayerUI()
    {
        uIDisplayed = true;

        groupStats.gameObject.SetActive(true);
        groupStats.FadeIn();
        groupAbilities.gameObject.SetActive(true);
        groupAbilities.FadeIn();
    }

    public void HidePlayerUI()
    {
        uIDisplayed = false;

        groupStats.gameObject.SetActive(true);
        groupStats.FadeOut();
        groupAbilities.gameObject.SetActive(true);
        groupAbilities.FadeOut();
    }
}