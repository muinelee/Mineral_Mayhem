using UnityEngine;
using System.Collections;

public class PlatformDrop : MonoBehaviour
{
    public float dropDistance = 50f;        // distance to drop *note* also affects speed that object is dropped
    public float dropDelay = 5f;           // delay before dropping
    public float destroyDelay = 5f;        // delay before destroying
    public float wallDestroyDelay = 2f;    // delay before destroying "SpawnWall" objects
    public float dropSpeed = 10f;          // speed of the drop

    public delegate void DropFinishedAction();
    public event DropFinishedAction OnFinishedDropping;

    void Start()
    {
        // starts the drop after the delay
        Invoke("StartDrop", dropDelay);
    }

    private void OnCollisionEnter(Collision other)
    {
        other.transform.SetParent(transform);
    }

    private void OnCollisionExit(Collision other)
    {
        other.transform.SetParent(null);
    }

    void StartDrop()
    {
        // initial position of the platform
        Vector3 initialPosition = transform.position;
        // calculates the distance in units set by dropDistance to get the target position
        Vector3 targetPosition = initialPosition - Vector3.up * dropDistance;
        // moves the platform
        StartCoroutine(DropPlatform(initialPosition, targetPosition));
    }

    IEnumerator DropPlatform(Vector3 initialPosition, Vector3 targetPosition)
    {
        // calculates drop duration
        float dropDuration = dropDistance / dropSpeed;

        // keeps track of time passed during drop
        float elapsedTime = 0f;

        while (elapsedTime < dropDuration)
        {
            // finds position based on time and speed
            float t = elapsedTime / dropDuration;
            Vector3 newPosition = Vector3.Lerp(initialPosition, targetPosition, t);

            // moves the platform 
            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // makes sure platform reaches the target position
        transform.position = targetPosition;

        if (OnFinishedDropping != null)
        {
            OnFinishedDropping();
        }

        // time before wall gets destroyed after drop
        yield return new WaitForSeconds(wallDestroyDelay);

        // finds and destroys objects tagged with "SpawnWall"
        GameObject[] spawnWalls = GameObject.FindGameObjectsWithTag("SpawnWall");
        foreach (GameObject spawnWall in spawnWalls)
        {
            Destroy(spawnWall);
        }

        // wait for the destroy delay
        //yield return new WaitForSeconds(destroyDelay);

        // destroy the platform
        //Destroy(gameObject);
    }
}
