using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class RocketLauncher : MonoBehaviour
{
    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private float rocketCooldown = 0.5f;
    [SerializeField] private float gravityWellCooldown = 2.0f;
    [SerializeField] private float recoilStrength = 1;
    [SerializeField] private UnityEvent onShoot = new UnityEvent();

    private bool canFireRocket = true;
    private bool canFireGravityWell = true;
    private Vector2 mouseDir = Vector2.right;

    private new Transform transform; 
    private YieldInstruction rocketCooldownInstruction;
    private YieldInstruction gravityWellCooldownInstruction;

    void Awake()
    {
        // Make sure the player reference is set, otherwise default to the root GameObject.
        CheckPlayer();

        transform = GetComponent<Transform>();
        rocketCooldownInstruction = new WaitForSeconds(rocketCooldown);
        gravityWellCooldownInstruction = new WaitForSeconds(gravityWellCooldown);
    }

    void Update()
    {
        // Update mouse direction
        mouseDir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

        // Fire rocket on click
        if (Input.GetButton("Shoot") && canFireRocket && !PauseMenu.Singleton.IsPaused)
        {
            // Start rocket cooldown
            StartCoroutine(RocketCooldown());

            // Spawn rocket
            Rocket rocket = PhotonNetwork.Instantiate("Rocket", transform.position, Quaternion.FromToRotation(Vector2.right, mouseDir), 0).GetComponent<Rocket>();
            rocket.IsLocal = true;
            rocket.Direction = mouseDir;

             // Apply recoil force to player
            playerBody.AddForce(recoilStrength * -mouseDir, ForceMode2D.Impulse);

            // Invoke onShoot event
            onShoot.Invoke();
        } else if(Input.GetButton("Alt Fire") && canFireGravityWell && !PauseMenu.Singleton.IsPaused)
        {
            StartCoroutine(GravityWellCooldown());

            GravityWell gravityWell = PhotonNetwork.Instantiate("GravityWell", transform.position, Quaternion.identity, 0).GetComponent<GravityWell>();
            gravityWell.IsLocal = true;
            gravityWell.Direction = mouseDir;

            onShoot.Invoke();
        }
    }

    void FixedUpdate()
    {
        // Look at the mouse position
        transform.rotation = Quaternion.FromToRotation(Vector2.right, mouseDir);
    }

    private IEnumerator RocketCooldown()
    {
        canFireRocket = false;
        yield return rocketCooldownInstruction;
        canFireRocket = true;
    }

    private IEnumerator GravityWellCooldown()
    {
        canFireGravityWell = false;
        yield return gravityWellCooldownInstruction;
        canFireGravityWell = true;
    }

    private void CheckPlayer()
    {
        if (playerBody == null)
        {
            Debug.LogWarning("Player reference not assigned to Rocket Launcher. Defaulting to root GameObject.");
            Rigidbody2D rootRB2D;
            if (transform.root.TryGetComponent<Rigidbody2D>(out rootRB2D))
            {
                playerBody = rootRB2D;
            }
            else
            {
                Debug.LogError("Failed to find valid root GameObject. Make sure the rocket launcher is a child of the player, and that the player has a Rigidbody2D component.");
            }
        }
    }
}
