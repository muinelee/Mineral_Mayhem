using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;

public class NetworkPlayer_Health : CharacterComponent
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    public float HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    private bool isInitialized = false;
    private Animator anim;

    [SerializeField] private float startingHP = 100;

    public NetworkPlayer.Team team;
    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetHealth(this);
    }

    // Start is called before the first frame update
    public override void Spawned()
    {
        if (HP == startingHP) isDead = false;
        anim = GetComponentInChildren<Animator>();

        RoundManager.Instance.ResetRound += Respawn;
        RoundManager.Instance.MatchEndEvent += DisableControls;
    }

    public override void OnHit(float x)
    {
        OnTakeDamage((int)x);
    }

    // Function only called on the server
    public void OnTakeDamage(int damageAmount)
    {
        if (damageAmount < 0)
        {
            Debug.Log("Damage is negative");
            return;
        }

        if (isDead)
        {
            return;
        }

        //Applies any damage reduction effects to the damage taken. currDamageAmount created to help with screenshake when being hit instead of adding the equation there
        int currDamageAmount = (int) (damageAmount * Character.StatusHandler.GetArmorValue());

        HP -= (float) currDamageAmount;

        //Debug.Log($"{Time.time} {transform.name} took damage and has {HP} HP left");

        if (HP <= 0)
        {
        //    Debug.Log($"{Time.time} {transform.name} is dead");

            isDead = true;
        }
        else NetworkCameraEffectsManager.instance.CameraHitEffect(currDamageAmount);
    }

    public void OnKnockBack(float force, Vector3 source)
    {
        if (isDead) return;

        source.y = transform.position.y;
        Vector3 direction = transform.position - source;

        Character.Rigidbody.Rigidbody.AddForce(direction * force);
    }

    private void HandleDeath()
    {
        DisableControls();

        anim.CrossFade("Death", 0.2f);

        if (!NetworkPlayer.Local.HasStateAuthority) return;
        if (team == NetworkPlayer.Team.Red) RoundManager.Instance.RedPlayersDies();
        else RoundManager.Instance.BluePlayersDies(); 
    }
    public void HandleRespawn()
    {
        EnableControls();
        anim.Play("Run");
    }

    static void OnHPChanged(Changed<NetworkPlayer_Health> changed)
    {
        //Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");
    }

    static void OnStateChanged(Changed<NetworkPlayer_Health> changed)
    {
        //Debug.Log($"{Time.time} OnStateChanged isDead {changed.Behaviour.isDead}");

        if (changed.Behaviour.isDead) changed.Behaviour.HandleDeath();
        else changed.Behaviour.HandleRespawn();
    }

    public float GetStartingHP()
    {
        return startingHP;
    }

    public void Heal(float amount)
    {
        HP = Mathf.Min(HP + amount, startingHP);
    }

    public void Respawn()
    {
        isDead = false;
        HP = startingHP;
    }

    public void DisableControls()
    {
        Character.Input.enabled = false;
        // Disable movement
        Character.Movement.enabled = false;
        // Disable attack
        Character.Attack.enabled = false;
        // Disable sphere collider
        Character.Collider.enabled = false;
        // Disable gravity
        Character.Rigidbody.Rigidbody.useGravity = false;
    }

    public void EnableControls()
    {
        Character.Input.enabled = true;
        // Disable movement
        Character.Movement.enabled = true;
        // Disable attack
        Character.Attack.enabled = true;
        // Disable sphere collider
        Character.Collider.enabled = true;
        // Disable gravity
        Character.Rigidbody.Rigidbody.useGravity = true;
    }

    public void OnDestroy()
    {
        RoundManager.Instance.ResetRound -= Respawn;
        RoundManager.Instance.MatchEndEvent -= DisableControls;
    }
}