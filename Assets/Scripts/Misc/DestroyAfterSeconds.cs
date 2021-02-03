using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float lifetime = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}
