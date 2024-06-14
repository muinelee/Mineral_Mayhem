using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacter : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 1f;

    //--------------------------------------//

    public void OnMouseDrag()
    {
        float XaxisRotation = Input.GetAxis("Mouse X") * -rotationSpeed;

        transform.Rotate(Vector3.up * XaxisRotation);
    }
}
