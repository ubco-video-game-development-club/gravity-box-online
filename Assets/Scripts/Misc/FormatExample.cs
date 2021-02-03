using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Example: rigidbody-based jumping script
public class FormatExample : MonoBehaviour
{
    // Example: inspector field not settable by other scripts
    [SerializeField] private int jumpSpeed;
    public int JumpSpeed { get { return jumpSpeed; } }

    // Example: private variable not gettable by other scripts
    private bool isGrounded;

    // Example: cached Component
    private Rigidbody2D rb2D;
    // Note: use transform directly

    void Awake()
    {
        // Variable initialization and component caching
    }

    void Start()
    {
        // Start-up functionality and object referencing
    }

    void Update()
    {
        // LINQ: good for select/filter/etc but use loops for iterating
    }
}
