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

    //Components
    private Animator anim;

    public override void Spawned()
    {
        if (!anim) anim = GetComponentInChildren<Animator>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData networkInputData) && canAttack)
        {
            if (networkInputData.isQAttack && !qAttackCoolDownTimer.IsRunning) ActivateAttack(qAttack, ref qAttackCoolDownTimer);
            if (networkInputData.isEAttack && !eAttackCoolDownTimer.IsRunning) ActivateAttack(eAttack, ref eAttackCoolDownTimer);
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
        canAttack = false;
        anim.SetBool("isAttacking", true);
        anim.CrossFade(attack.attackName, 0.2f);
        attackTimer = TickTimer.CreateFromSeconds(Runner, attack.GetCoolDown());
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public void FireQAttack()
    {
        Runner.Spawn(qAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public void FireEAttack()
    {
        Runner.Spawn(eAttack.GetAttackPrefab(), transform.position + Vector3.up, transform.rotation, Object.InputAuthority);
    }

    public SO_NetworkAttack GetQAttack()
    {
        return qAttack;
    }

    public SO_NetworkAttack GetEAttack()
    {
        return eAttack;
    }

    public ref TickTimer GetQAttackCoolDownTimer()
    {
        return ref qAttackCoolDownTimer;
    }

    public ref TickTimer GetEAttackCoolDownTimer()
    {
        return ref eAttackCoolDownTimer;
    }

    public void ResetCanAttack()
    {
        canAttack = true;
    }
}