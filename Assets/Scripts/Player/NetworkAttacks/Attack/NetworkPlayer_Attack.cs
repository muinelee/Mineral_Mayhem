using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.UIElements;
using static UnityEngine.CullingGroup;

public class NetworkPlayer_Attack : CharacterComponent
{
    // Control variables
    public bool canAttack = true;
    [Networked(OnChanged = nameof(OnStateChanged))] public NetworkBool isDefending { get; set; } = false;

    [Header("Basic Attacks Properties")]
    [SerializeField] private SO_NetworkBasicAttack[] basicAttacks;
    private bool canBasicAttack = true;
    private int basicAttackCount = 0;

    [Header("Q Attack Properties")]
    [SerializeField] private SO_NetworkAttack qAttack;
    private TickTimer qAttackCoolDownTimer;
    private Queue<AudioClip> qVoiceLineQueue = new Queue<AudioClip>();

    [Header("E Attack Properties")]
    [SerializeField] private SO_NetworkAttack eAttack;
    private TickTimer eAttackCoolDownTimer;
    private Queue<AudioClip> eVoiceLineQueue = new Queue<AudioClip>();

    [Header("(Ult) F Attack Properties")]
    [SerializeField] private SO_NetworkUlt fAttack;
    private Queue<AudioClip> fVoiceLineQueue = new Queue<AudioClip>();

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetAttack(this);
        InitializeVoiceLineQueue(qAttack.GetVoiceLine(), qVoiceLineQueue);
        InitializeVoiceLineQueue(eAttack.GetVoiceLine(), eVoiceLineQueue);
        InitializeVoiceLineQueue(fAttack.GetVoiceLine(), fVoiceLineQueue);
    }

    private void InitializeVoiceLineQueue(AudioClip[] voiceLines, Queue<AudioClip> voiceLineQueue)
    {
        if (voiceLines != null)
        {
            foreach (AudioClip voiceLine in voiceLines)
            {
                voiceLineQueue.Enqueue(voiceLine);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input) && canAttack)
        {

            if (input.IsDown(NetworkInputData.ButtonF) && Character.Energy.IsUltCharged() && !isDefending) ActivateUlt();
            else if (input.IsDown(NetworkInputData.ButtonQ) && !qAttackCoolDownTimer.IsRunning && !isDefending) ActivateAttack(ref qAttack);
            else if (input.IsDown(NetworkInputData.ButtonE) && !eAttackCoolDownTimer.IsRunning && !isDefending) ActivateAttack(ref eAttack);
            else if (input.IsDown(NetworkInputData.ButtonBasic) && basicAttackCount < basicAttacks.Length && canBasicAttack && !isDefending) ActivateBasicAttack();
            ActivateBlock(input.IsDown(NetworkInputData.ButtonBlock));
        }

        ManageTimers(ref qAttackCoolDownTimer);
        ManageTimers(ref eAttackCoolDownTimer);
    }

    private void ManageTimers(ref TickTimer timer)
    {
        if (timer.Expired(Runner)) timer = TickTimer.None;
    }

    private void ActivateAttack(ref SO_NetworkAttack attack)
    {
        if (Runner.IsServer == false) return;

        // Start Attack animation
        canAttack = false;

        if (attack == qAttack)
        {
            RPC_PlayQEffects();
            StartQAttackCooldown();
            RPC_StartCoolDownTimer(true);
        }

        else
        {
            RPC_PlayEEffects();
            StartEAttackCooldown();
            RPC_StartCoolDownTimer(false);
        }

        // Slow player
        Character.Movement.ApplyAbility(attack);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    public void RPC_StartCoolDownTimer(bool isQAttack)
    {
        if (isQAttack) this.StartQAttackCooldown();
        else this.StartEAttackCooldown();
    }

    public void StartQAttackCooldown()
    {
        qAttackCoolDownTimer = TickTimer.CreateFromSeconds(Runner, qAttack.GetCoolDown());
    }

    public void StartEAttackCooldown()
    {
        eAttackCoolDownTimer = TickTimer.CreateFromSeconds(Runner, eAttack.GetCoolDown());
    }

    private void ActivateUlt()
    {
        if (Runner.IsServer == false) return;

        // Start Ult animation
        canAttack = false;
        Character.Energy.energy = 0;
        RPC_PlayFEffects();

        // Slow player
        Character.Movement.ApplyAbility(fAttack);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayQEffects()
    {
        this.PlayQVoiceLine();
        Character.Animator.anim.CrossFade(qAttack.attackName, 0.2f);
        Character.Animator.anim.CrossFade("Helper", 0.2f, 2);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayEEffects()
    {
        this.PlayEVoiceLine();
        Character.Animator.anim.CrossFade(eAttack.attackName, 0.2f);
        Character.Animator.anim.CrossFade("Helper", 0.2f, 2);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayFEffects()
    {
        this.PlayFVoiceLine();
        Character.Animator.anim.CrossFade(fAttack.attackName, 0.1f);
        Character.Animator.anim.CrossFade("Helper", 0.2f, 2);
    }

    public void PlayQVoiceLine()
    {
        PlayVoiceLine(qVoiceLineQueue);
    }

    public void PlayEVoiceLine()
    {
        PlayVoiceLine(eVoiceLineQueue);
    }

    public void PlayFVoiceLine()
    {
        PlayVoiceLine(fVoiceLineQueue);
    }

    private void PlayVoiceLine(Queue<AudioClip> voiceLineQueue)
    {
        if (voiceLineQueue != null && voiceLineQueue.Count > 0)
        {
            AudioClip voiceLine = voiceLineQueue.Dequeue();
            AudioManager.Instance.PlayAudioSFX(voiceLine, transform.localPosition);
            voiceLineQueue.Enqueue(voiceLine);
        }
    }

    private void ActivateBasicAttack()
    {
        canBasicAttack = false;

        if (basicAttackCount == 0)
        {
            Character.Animator.anim.CrossFade(basicAttacks[basicAttackCount].attackName, 0.1f);
            Character.Animator.anim.CrossFade("Helper", 0.2f, 2);
            Character.Movement.ApplyAbility(basicAttacks[basicAttackCount]);
        }
    }

    public void AllowChainBasicAttack()
    {
        canBasicAttack = true;
    }

    public void AttackMomentum()
    {
        if (!Runner.IsServer) return;

        Character.Rigidbody.Rigidbody.AddForce(transform.forward * basicAttacks[basicAttackCount].momentum, ForceMode.Impulse);
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
            if (Object.HasStateAuthority)
            {
                Character.OnBlock(true);
            }
        }
        else if (!blockButtonDown && isDefending)
        {
            if (Object.HasStateAuthority)
            {
                Character.OnBlock(false);
            }
        }
    }

    public override void OnBlock(bool isBlocking)
    {
        isDefending = isBlocking;
        Assert.Check(Character.Shield);
        Character.Shield.gameObject.SetActive(isBlocking);
    }

    // Needs to be linked via NetworkPlayer_AnimationLink Script
    public void FireQAttack()
    {
        if (!Object.HasStateAuthority) return;

        Runner.Spawn(qAttack.GetAttackPrefab(), transform.position, transform.rotation, Object.InputAuthority);
    }

    public void FireEAttack()
    {
        if (!Object.HasStateAuthority) return;
        
        Runner.Spawn(eAttack.GetAttackPrefab(), transform.position, transform.rotation, Object.InputAuthority);
    }

    public void FireFAttack()
    {
        if (!Object.HasStateAuthority) return;
            
        Runner.Spawn(fAttack.GetAttackPrefab(), transform.position, transform.rotation, Object.InputAuthority);
    }

    public void FireBasicAttack()
    {
        if (Object.HasStateAuthority) Runner.Spawn(basicAttacks[basicAttackCount].GetAttackPrefab(), transform.position, transform.rotation, Object.InputAuthority);
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

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private static void OnStateChanged(Changed<NetworkPlayer_Attack> changed)
    {
        changed.LoadNew();
        changed.Behaviour.Character.Shield.gameObject.SetActive(changed.Behaviour.isDefending);
    }
}