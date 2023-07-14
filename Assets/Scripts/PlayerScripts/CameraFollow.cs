using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Object to be tracked.
    public float distanceMultiplier; // The multiplier that affects the camera's retraction.
    public float smoothTime = 0.3f; // Camera smooth time;
    private Vector3 velocity = Vector3.zero; 
    [SerializeField] private Vector3 offset; // Camera offset.


    private void LateUpdate()
    {
        if (target == null) return;

        // As the target's scale increases, the camera position is re-adjusted.
        // This prevents the camera from being too close to the target.  
        Vector3 desiredPosition = target.position + offset + 
            new Vector3(0, target.transform.localScale.x * distanceMultiplier, -target.transform.localScale.x * distanceMultiplier);

        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
