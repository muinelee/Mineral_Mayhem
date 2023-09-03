using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    public PlayerHealth_Test_Ivan playerHealthScript; // Reference to your PlayerHealth_Test_Ivan script

    // This function is called when the player collides with another object
    void OnCollisionEnter(Collision collision)
    {
        // Check if the player collided with the test dummy
        if (collision.gameObject.tag == "NPC")
        {
            // Call the TakeDamage method from your PlayerHealth_Test_Ivan script
            playerHealthScript.TakeDamage(10);
        }
    }
}