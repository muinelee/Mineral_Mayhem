using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private Vector3 offset;
    [SerializeField] 
    private Transform target;
    [SerializeField] 
    private float smoothTime;
    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        offset = transform.position - target.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}
