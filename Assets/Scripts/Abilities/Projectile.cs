using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  public GameObject firePoint; 

  public List<GameObject> vfx = new List<GameObject>(); 
  public RotateToMouse rotateToMouse; 

  

  private GameObject effectToSpawn; 
  private float timeToFire = 0; 

  public float timeBtwShots = 2;
  private float timeOfLastshot = 0f; 
 
  
  void Start()
  {
    effectToSpawn = vfx [0]; 
  }

  void Update()
  {

    if(Input.GetMouseButton(1) && Time.time >= timeToFire)
    {
       timeToFire = Time.time + 1 /effectToSpawn.GetComponent<ProjectileMovement> ().fireRate;
       SpawnVFX (); 
    }
    if(Input.GetMouseButtonDown(1))
    {
   
      if (Time.deltaTime - timeOfLastshot >= timeBtwShots)
      {
        SpawnVFX();
        timeOfLastshot = Time.deltaTime;
      } 
    }

    void SpawnVFX()
    {
        GameObject vfx; 
        
        if(firePoint != null)
        {
            vfx = Instantiate (effectToSpawn, firePoint.transform.position, Quaternion.identity); 
            if(rotateToMouse != null)
            {
              vfx.transform.localRotation = rotateToMouse.GetRotation();
            }
        }
        else
        {
            Debug.Log("No Fire Point"); 
        }
    }
  }
}

