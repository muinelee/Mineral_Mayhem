using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer_Attack : NetworkBehaviour
{
    // Control variables
    private bool canAttack = true;

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
            if (networkInputData.isQAttack && !qAttackCoolDownTimer.IsRunning) ActivateAttack(qAttack, ref qAttackCoolDownTimer);
            if (networkInputData.isEAttack && !eAttackCoolDownTimer.IsRunning) ActivateAttack(eAttack, ref eAttackCoolDownTimer);
            if (networkInputData.isFAttack && playerEnergy.IsUltCharged()) ActivateAttack(fAttack);
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
        playerMovement.SetTurnSlow(attack.GetTurnSlow());
        playerMovement.SetAbilitySlow(attack.GetAbilitySlow());
    }

    private void ActivateAttack(SO_NetworkUlt ult)
    {
        // Start Ult animation
        canAttack = false;
        anim.CrossFade(ult.attackName, 0.2f);

        // Slow player
        playerMovement.SetTurnSlow(ult.GetTurnSlow());
        playerMovement.SetAbilitySlow(ult.GetAbilitySlow());
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    // Needs to be linked via NetworkPlayer_AnimationLink Script
    public void FireQAttack()
    {
        if (Object.HasStateAuthority) Runner.Spawn(qAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public void FireEAttack()
    {
        if (Object.HasStateAuthority) Runner.Spawn(eAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public void FireFAttack()
    {
        if (Object.HasStateAuthority) Runner.Spawn(fAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
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
        playerMovement.ResetTurnSlow();
        playerMovement.ResetAbilitySlow();
    }
}