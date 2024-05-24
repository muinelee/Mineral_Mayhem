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
    private List<int> qVoiceLineIndex = new List<int>();

    [Header("E Attack Properties")]
    [SerializeField] private SO_NetworkAttack eAttack;
    private TickTimer eAttackCoolDownTimer;
    private List<int> eVoiceLineIndex = new List<int>();

    [Header("(Ult) F Attack Properties")]
    [SerializeField] private SO_NetworkUlt fAttack;
    private List<int> fVoiceLineIndex = new List<int>();

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetAttack(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input) && canAttack)
        {

            if (input.IsDown(NetworkInputData.ButtonF) && Character.Energy.IsUltCharged() && !isDefending) ActivateUlt();
            else if (input.IsDown(NetworkInputData.ButtonQ) && !qAttackCoolDownTimer.IsRunning && !isDefending) ActivateAttack(ref qAttack, true);
            else if (input.IsDown(NetworkInputData.ButtonE) && !eAttackCoolDownTimer.IsRunning && !isDefending) ActivateAttack(ref eAttack, false);
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

    private void ActivateAttack(ref SO_NetworkAttack attack, bool isQAttack)
    {
        if (Runner.IsServer == false) return;

        // Start Attack animation
        canAttack = false;

        if (attack == qAttack) RPC_PlayQEffects();
        else RPC_PlayEEffects();

        // if isQAttack is false, it is  the eAttack
        if (isQAttack)
        {
            StartQAttackCooldown();
            RPC_StartCoolDownTimer(true);
        }
        else
        {
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
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayEEffects()
    {
        this.PlayEVoiceLine();
        Character.Animator.anim.CrossFade(eAttack.attackName, 0.2f);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_PlayFEffects()
    {
        this.PlayFVoiceLine();
        Character.Animator.anim.CrossFade(fAttack.attackName, 0.1f);
    }

    public void PlayQVoiceLine()
    {
        PlayVoiceLine(qAttack.GetVoiceLine(), ref qVoiceLineIndex);
    }

    public void PlayEVoiceLine()
    {
        PlayVoiceLine(eAttack.GetVoiceLine(), ref eVoiceLineIndex);
    }

    public void PlayFVoiceLine()
    {
        PlayVoiceLine(fAttack.GetVoiceLine(), ref fVoiceLineIndex);
    }

    private void PlayVoiceLine(AudioClip[] voiceLines, ref List<int> voiceLineIndices)
    {
        if (voiceLines != null && voiceLines.Length > 0)
        {
            if (voiceLineIndices.Count == 0)
            {
                for (int i = 0; i < voiceLines.Length; i++)
                {
                    voiceLineIndices.Add(i);
                }
            }

            int randomIndex = Random.Range(0, voiceLineIndices.Count);
            int voiceLineIndex = voiceLineIndices[randomIndex];
            voiceLineIndices.RemoveAt(randomIndex);

            AudioClip randomVoiceLine = voiceLines[voiceLineIndex];
            AudioManager.Instance.PlayAudioSFX(randomVoiceLine, transform.localPosition);
        }
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