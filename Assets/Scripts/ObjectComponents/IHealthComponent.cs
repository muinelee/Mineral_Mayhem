using Fusion;
using UnityEngine;

public interface IHealthComponent
{
    [Networked] public float HP { get; set; }

    [Networked] public bool isDead { get; set; }

    public NetworkPlayer.Team team { get; set; }

    // Function for when object dies
    public void HandleDeath() { }

    // Function for if object respawns
    public void HandleRespawn() { }

    // Function for when taking damage
    public void OnTakeDamage(float damageAmount, bool playReact) { }

    // Function for if the object can get knockedback
    public void OnKnockBack(float force, Vector3 source) { }

    public NetworkPlayer.Team GetTeam()
    {
        return team;
    }
}