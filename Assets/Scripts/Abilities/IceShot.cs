
using UnityEngine;

using System.Collections;



public class IceShot: MonoBehaviour

{

    public GameObject projectile; 
    public float launchVelocity = 800f; 
    //[SerializeField] float fireRate 5f; 
    
    void Update()
    {
        if(Input.GetButtonDown("Fire2"))
        {
          GameObject launchedObject = Instantiate(projectile, transform.position, transform.rotation); 
          launchedObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3 (0, launchVelocity, 0)); 
        }
    

    

   {
        if (Input.GetButton("Fire2") && GetComponent<BoxCollider>())
        {
            Destroy(GetComponent<BoxCollider>());
        }
    }
    } 
       // public IEnumberator RapidFire()
       // {
           // while (true)
           // {
               //// Shoot(); 
               /////// yield return new WaitForSeonds(1 / fireRate); 
          //  }

       // }

    }


