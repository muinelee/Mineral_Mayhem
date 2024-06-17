using UnityEngine;
using Fusion;
using System.Collections;

public class NetworkPlayer_Health : CharacterComponent, IHealthComponent
{
    [Networked(OnChanged = nameof(OnHPChanged))]
    public float HP { get; set; }

    // Callback is Temporary until OnHPChanged is implemented
    [Networked(OnChanged = nameof(OnHPChanged))]
    public float BP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]
    public bool isDead { get; set; }

    public NetworkPlayer.Team team { get; set; }

    private bool isInitialized = false;

    // Health Properties
    public float startingHP = 100;

    // Block Properties
    [SerializeField] private float startingBP = 100;
    [SerializeField] private float blockDamageReduction = 0.5f;
    [SerializeField] private StatusEffect blockDepletedStun;    // Call if block is 0
    [SerializeField] private float blockDrainRate = 10.0f;      // Can change for balancing
    [SerializeField] private float blockRechargeRate = 10.0f;   // Can change for balancing
    public bool canBlock = true;

    // Team Cam Properties
    [SerializeField] private float timeUntilTeamCam = 5;
    [SerializeField] private TickTimer teamCamTimer = TickTimer.None;

    [SerializeField] private Material shaderGraphMaterial;
    private Material materialInstance;

    private ShowOverlays overlays;

    public override void Init(CharacterEntity character)
    {
        base.Init(character);
        character.SetHealth(this);
        PrimeShieldMaterial();
    }

    public override void FixedUpdateNetwork()
    {
        HandleBlockMeter();
        HandleTeamCam();
        HandleBlockVisual();
    }

    // Start is called before the first frame update
    public override void Spawned()
    {
        if (HP == startingHP) isDead = false;
        RoundManager rm = RoundManager.Instance;
        if (rm != null)
        {
            rm.ResetRound += Respawn;
            rm.MatchEndEvent += DisableControls;
        }
        else
        {
            Respawn();
        }

        overlays = FindObjectOfType<ShowOverlays>();
        overlays.ShowHealthOverlay(HP / startingHP);
    }

    private void PrimeShieldMaterial()
    {
        if (!shaderGraphMaterial) return;
        materialInstance = new Material(shaderGraphMaterial);
        Renderer renderer = Character.Shield.GetComponent<Renderer>();
        if (renderer) renderer.material = materialInstance;
        else Debug.Log($"No Renderer on Shield found for {gameObject.name}");
    }

    public override void OnHit(float x, bool hitReact)
    {
        if (x < 0 || isDead) return;

        //Applies any damage reduction effects to the damage taken. currDamageAmount created to help with screenshake when being hit instead of adding the equation there
        float currDamageAmount = x * Character.StatusHandler.GetDamageReduction();

        if (Character.Attack.isDefending)
        {
            float blockDamageAmount = (currDamageAmount * (blockDamageReduction));
            BP = Mathf.Max(BP - blockDamageAmount, 0);
            currDamageAmount = (currDamageAmount * (1 - blockDamageReduction));
        }

        HP -= currDamageAmount;

        if (HP <= 0)
        {
            isDead = true;
        }
        else
        {
            NetworkCameraEffectsManager.instance.CameraHitEffect(currDamageAmount);
            if (hitReact) RPC_PlayHitAnimation();
        }

        RPC_HealthOverlay();
    }

    public void HandleBlockMeter()
    {
        if (!Character.Attack.isDefending && BP < startingBP)
        {
            BP += blockRechargeRate * Runner.DeltaTime;
            BP = Mathf.Clamp(BP, 0, startingBP);
            if (BP >= startingBP) canBlock = true;
        }
        else if (Character.Attack.isDefending && canBlock)
        {
            BP -= blockDrainRate * Runner.DeltaTime;
            if (BP <= 0)
            {
                BP = 0;
                HandleBlockDepletion();
            }
        }
    }

    private void HandleBlockDepletion()
    {
        if (!Runner.IsServer) return;

        canBlock = false;
        Character.OnStatusBegin(blockDepletedStun);
    }

    private void HandleBlockVisual()
    {
        if (materialInstance)
        {
            materialInstance.SetFloat("_alpha", BP / startingBP);
        }
    }

    private void HandleTeamCam()
    {
        if (!Object.HasInputAuthority) return;

        if (HP > 0)
        {
            if (teamCamTimer.IsRunning) teamCamTimer = TickTimer.None;
            return;
        }

        if (teamCamTimer.Expired(Runner) && Runner.SessionInfo.MaxPlayers > 2)
        {
            teamCamTimer = TickTimer.None;
            NetworkCameraEffectsManager.instance.GoToTeamCamera();
        }
    }

    // Function only called on the server
    public void OnTakeDamage(float damageAmount, bool playReact)
    {
        Character.OnHit(damageAmount, playReact);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_PlayHitAnimation()
    {
        Character.Animator.anim.Play("Hit", 1);
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

        RPC_HealthOverlay();
        RPC_RemoveStormOverlay();
        TimerManager.instance.StopTimer(false);

        teamCamTimer = TickTimer.CreateFromSeconds(Runner, timeUntilTeamCam);

        Character.Animator.anim.Play("Death");
        Character.Animator.anim.Play("Death", 2);

        if (!NetworkPlayer.Local.HasStateAuthority)
        {
            return;
        }

        if (RoundManager.Instance)
        {
            if (team == NetworkPlayer.Team.Red) RoundManager.Instance.RedPlayersDies();
            else RoundManager.Instance.BluePlayersDies();
        }


    }
    public void HandleRespawn()
    {
        RPC_HealthOverlay();
        RPC_RemoveStormOverlay();
        TimerManager.instance.ResetTimer(0);

        Character.Animator.anim.Play("Run");
        Character.Animator.anim.Play("Run", 2);
        Character.Rigidbody.Rigidbody.velocity = Vector3.zero;
        if (Object.HasInputAuthority) NetworkCameraEffectsManager.instance.GoToTopCamera();
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
        RPC_HealthOverlay();
    }

    public void Respawn()
    {
        isDead = false;
        HP = startingHP;
    }

    public void DisableControls()
    {
        Character.Attack.ActivateBlock(false);
        // Disable Input
        Character.Input.enabled = false;
        // Disable movement
        Character.Movement.enabled = false;
        // Disable attack
        Character.Attack.enabled = false;
        // Disable sphere collider
        Character.Collider.enabled = false;

        Character.Animator.anim.SetFloat("Xaxis", 0);
        Character.Animator.anim.SetFloat("Zaxis", 0);
    }

    public void EnableControls()
    {
        Character.Energy.energy = 0;

        // Use helper to prevent player to from using ult on first frame
        StartCoroutine(EnableControlHelper());
    }

    IEnumerator EnableControlHelper()
    {
        yield return 0;

        // Enable Input
        Character.Input.enabled = true;
        // Enable movement
        Character.Movement.enabled = true;
        // Enable attack
        Character.Attack.enabled = true;
        Character.Attack.ResetAttackCapabilities();
        // Enable sphere collider
        Character.Collider.enabled = true;
        // Enable gravity
        Character.Rigidbody.Rigidbody.useGravity = true;
    }

    public void OnDestroy()
    {
        if (RoundManager.Instance)
        {
            RoundManager.Instance.ResetRound -= Respawn;
            RoundManager.Instance.MatchEndEvent -= DisableControls;
        }
    }

    public override void OnHeal(float x)
    {
        Heal(x);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_HealthOverlay()
    {
        if (NetworkPlayer.Local.Object.InputAuthority == this.Object.InputAuthority)
            overlays.ShowHealthOverlay(HP / startingHP);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RemoveStormOverlay()
    {
        if (NetworkPlayer.Local.Object.InputAuthority == this.Object.InputAuthority)
            overlays.OnEnterStorm();
    }
}