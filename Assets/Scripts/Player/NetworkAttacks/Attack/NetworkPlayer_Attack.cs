using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Security.Cryptography.X509Certificates;

public class NetworkPlayer_Attack : CharacterComponent
{
    // Control variables
    public bool canAttack = true;
    [Networked] public bool isDefending { get; set; } = false;

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

    [Header("Block Properties")]
    [SerializeField] private NetworkObject blockShield;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetAttack(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input) && canAttack)
        {
            if (input.IsDown(NetworkInputData.ButtonF) && Character.Energy.IsUltCharged()) ActivateUlt();
            else if (input.IsDown(NetworkInputData.ButtonQ) && !qAttackCoolDownTimer.IsRunning) ActivateAttack(qAttack, ref qAttackCoolDownTimer);
            else if (input.IsDown(NetworkInputData.ButtonE) && !eAttackCoolDownTimer.IsRunning) ActivateAttack(eAttack, ref eAttackCoolDownTimer);
            else if (input.IsDown(NetworkInputData.ButtonBasic) && basicAttackCount < basicAttacks.Length && canBasicAttack) ActivateBasicAttack();
            ActivateBlock(input.IsDown(NetworkInputData.ButtonBlock));
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
        Character.Animator.anim.CrossFade(attack.attackName, 0.2f);
        attackTimer = TickTimer.CreateFromSeconds(Runner, attack.GetCoolDown());

        // Slow player
        Character.Movement.ApplyAbility(attack);
    }

    private void ActivateUlt()
    {
        // Start Ult animation
        canAttack = false;
        Character.Animator.anim.CrossFade(fAttack.attackName, 0.2f);

        // Slow player
        Character.Movement.ApplyAbility(fAttack);
    }

    private void ActivateBasicAttack()
    {
        canBasicAttack = false;

        if (basicAttackCount == 0)
        {
            Character.Animator.anim.CrossFade(basicAttacks[basicAttackCount].attackName, 0.1f);
            Character.Movement.ApplyAbility(basicAttacks[basicAttackCount]);
        }
    }

    public void AllowChainBasicAttack()
    {
        canBasicAttack = true;
    }

    public void ChainBasicAttack()
    {
        if (canBasicAttack) return;

        Character.Animator.anim.CrossFade(basicAttacks[basicAttackCount].attackName, 0.1f);
        Character.Movement.ApplyAbility(basicAttacks[basicAttackCount]);
    }

    public void ActivateBlock(bool blockButtonDown)
    {
        if (blockButtonDown && !isDefending && Character.Health.canBlock)
        {
            Runner.Spawn(blockShield, transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
            Character.OnBlock(true);
            Character.Animator.anim.CrossFade("Block", 0.1f);
        }
        else if (!blockButtonDown && isDefending)
        {
            Runner.Despawn(blockShield);
            Character.OnBlock(false);
            Character.Animator.ResetAnimation();
        }
    }

    public override void OnBlock(bool isBlocking)
    {
        isDefending = isBlocking;
    }

    // Needs to be linked via NetworkPlayer_AnimationLink Script
    public void FireQAttack()
    {
        if (!Object.HasStateAuthority) return;

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
        if (Object.HasStateAuthority) Runner.Spawn(basicAttacks[basicAttackCount].GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
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
        Character.Movement.ResetSlows();
    }
}