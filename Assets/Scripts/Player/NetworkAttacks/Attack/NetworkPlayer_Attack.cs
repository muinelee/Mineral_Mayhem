using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer_Attack : CharacterComponent
{
    // Control variables
    public bool canAttack = true;

    [Header("Basic Attacks Properties")]
    [SerializeField] private SO_NetworkBasicAttack[] basicAttacks;
    private bool canBasicAttack = true;
    private int basicAttackCount = 0;

    [Header("Q Attack Properties")]
    [SerializeField] private SO_NetworkAttack qAttack;
    private TickTimer qAttackCoolDownTimer;

    [Header("E Attack Properties")]
    [SerializeField] private SO_NetworkAttack eAttack;
    private TickTimer eAttackCoolDownTimer;

    [Header("(Ult) F Attack Properties")]
    [SerializeField] private SO_NetworkUlt fAttack;

    //Components
    private Animator anim;
    private NetworkPlayer_Energy playerEnergy;
    private NetworkPlayer_Movement playerMovement;
    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetAttack(this);
    }

    public override void Spawned()
    {
        anim = GetComponentInChildren<Animator>();
        playerEnergy = GetComponent<NetworkPlayer_Energy>();
        playerMovement = GetComponent<NetworkPlayer_Movement>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData) && canAttack)
        {
            if (networkInputData.isFAttack && playerEnergy.IsUltCharged()) ActivateUlt();
            else if (networkInputData.isQAttack && !qAttackCoolDownTimer.IsRunning) ActivateAttack(qAttack, ref qAttackCoolDownTimer);
            else if (networkInputData.isEAttack && !eAttackCoolDownTimer.IsRunning) ActivateAttack(eAttack, ref eAttackCoolDownTimer);
            else if (networkInputData.isBasicAttack && basicAttackCount < basicAttacks.Length && canBasicAttack) ActivateBasicAttack();
        }

        ManageTimers(ref qAttackCoolDownTimer);
        ManageTimers(ref eAttackCoolDownTimer);
    }

    private void ManageTimers(ref TickTimer timer)
    {
        if (timer.Expired(Runner)) timer = TickTimer.None;
    }

    private void ActivateAttack(SO_NetworkAttack attack, ref TickTimer attackTimer)
    {
        // Start Attack animation
        canAttack = false;
        anim.CrossFade(attack.attackName, 0.2f);
        attackTimer = TickTimer.CreateFromSeconds(Runner, attack.GetCoolDown());

        // Slow player
        playerMovement.ApplyAbility(attack);
    }

    private void ActivateUlt()
    {
        // Start Ult animation
        canAttack = false;
        anim.CrossFade(fAttack.attackName, 0.2f);

        // Slow player
        playerMovement.ApplyAbility(fAttack);
    }

    private void ActivateBasicAttack()
    {
        canBasicAttack = false;

        if (basicAttackCount == 0)
        {
            anim.CrossFade(basicAttacks[basicAttackCount].attackName, 0.1f);
            playerMovement.ApplyAbility(basicAttacks[basicAttackCount]);
        }
    }

    public void AllowChainBasicAttack()
    {
        canBasicAttack = true;
    }

    public void ChainBasicAttack()
    {
        if (canBasicAttack) return;

        anim.CrossFade(basicAttacks[basicAttackCount].attackName, 0.1f);
        playerMovement.ApplyAbility(basicAttacks[basicAttackCount]);
    }

    // Needs to be linked via NetworkPlayer_AnimationLink Script
    public void FireQAttack()
    {
        if (Object.HasStateAuthority) return;
            
        Runner.Spawn(qAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public void FireEAttack()
    {
        if (!Object.HasStateAuthority) return;
        
        Runner.Spawn(eAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public void FireFAttack()
    {
        if (!Object.HasStateAuthority) return;
            
        Runner.Spawn(fAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public void FireBasicAttack()
    {
        if (!Object.HasStateAuthority) return;

        Runner.Spawn(basicAttacks[basicAttackCount].GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
        basicAttackCount++;
    }

    public SO_NetworkAttack GetQAttack()
    {
        return qAttack;
    }

    public SO_NetworkAttack GetEAttack()
    {
        return eAttack;
    }

    public SO_NetworkUlt GetFAttack()
    {
        return fAttack;
    }

    public SO_NetworkBasicAttack GetBasicAttack(int index)
    {
        return basicAttacks[index];
    }

    public float GetQAttackCoolDownTimer()
    {
        if (qAttackCoolDownTimer.IsRunning) return (float)qAttackCoolDownTimer.RemainingTime(Runner);
        else return 0;
    }

    public float GetEAttackCoolDownTimer()
    {
        if (eAttackCoolDownTimer.IsRunning) return (float)eAttackCoolDownTimer.RemainingTime(Runner);
        else return 0;
    }

    public void ResetAttackCapabilities()
    {
        canAttack = true;
        canBasicAttack = true;
        basicAttackCount = 0;
        playerMovement.ResetSlows();
    }
}