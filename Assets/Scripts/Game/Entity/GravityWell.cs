using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    [SerializeField] private float gravitationalAcceleration;
    [SerializeField] private float timeToLive;
    private Rigidbody2D[] trackedBodies;
    private YieldInstruction waitForSeconds;

    void Awake()
    {
        waitForSeconds = new WaitForSeconds(1.0f);
        if(timeToLive > 0.0f) Destroy(gameObject, timeToLive);
    }

    void OnEnable()
    {
        StartCoroutine(UpdateTrackedBodies());
    }

    void Update()
    {
        foreach(Rigidbody2D rig in trackedBodies)
        {
            if(rig == null) continue;

            Vector2 d = (Vector2)transform.position - rig.position;
            Vector2 dir = d.normalized;
            float ds = d.sqrMagnitude + 1.0f;

            float a = gravitationalAcceleration / ds;
            rig.AddForce(dir * a * rig.mass);
        }
    }

    private IEnumerator UpdateTrackedBodies()
    {
        while(enabled)
        {
            trackedBodies = FindObjectsOfType<Rigidbody2D>();
            yield return waitForSeconds;
        }
    }
}
