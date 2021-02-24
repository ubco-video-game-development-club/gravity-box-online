using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float dampTime = 0.3f;

    private bool isFollowing = false;
    private Vector2 currentVelocity;
    private Transform target;

    void FixedUpdate()
    {
        if (isFollowing)
        {
            Vector2 targetPos = Vector2.SmoothDamp(transform.position, target.position, ref currentVelocity, dampTime);
            transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        }
    }

    public void FollowTarget(Transform target)
    {
        this.target = target;
        isFollowing = true;
    }
}
