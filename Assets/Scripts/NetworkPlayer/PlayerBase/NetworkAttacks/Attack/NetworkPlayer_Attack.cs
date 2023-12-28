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

    public override void Spawned()
    {
        anim = GetComponentInChildren<Animator>();
        playerEnergy = GetComponent<NetworkPlayer_Energy>();
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
        canAttack = false;
        anim.SetBool("isAttacking", true);
        anim.CrossFade(attack.attackName, 0.2f);
        attackTimer = TickTimer.CreateFromSeconds(Runner, attack.GetCoolDown());
    }

    private void ActivateAttack(SO_NetworkUlt ult)
    {
        Debug.Log("BIG BANG ATTACKKKKK");

        //canAttack = false;
        //anim.CrossFade(ult.attackName, 0.2f);
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

    public SO_NetworkUlt GetFAttack()
    {
        return fAttack;
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