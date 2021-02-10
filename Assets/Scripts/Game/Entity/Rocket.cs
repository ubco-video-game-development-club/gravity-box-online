using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rocket : MonoBehaviourPun
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private GameObject explosionTextPrefab;
    [SerializeField] private float speed = 1;
    [SerializeField] private float explosionStrength = 1;
    [SerializeField] private float explosionRadius = 1;
    [SerializeField] private float minDamage = 3.0f;
    [SerializeField] private float maxDamage = 10.0f;

	public bool IsLocal { get; set; }

    public Vector2 Direction
    {
        get { return direction; }
        set
        {
            direction = value.normalized;
            ApplyVelocity(direction * speed);
        }
    }
    private Vector2 direction = Vector2.right;

    private new Rigidbody2D rigidbody2D;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

	[PunRPC]
	public void Explode()
	{
		// Spawn explosion effect
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Instantiate(explosionTextPrefab, transform.position, Quaternion.identity, HUD.Singleton.ExplosionTextParent);
	}

    private void ApplyVelocity(Vector2 velocity)
    {
		if(!IsLocal) return;
        transform.rotation = Quaternion.FromToRotation(Vector2.right, velocity);
        rigidbody2D.velocity = velocity;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
		if(!IsLocal) return;

        // Get enemy hit if we did direct hit
        Player hitPlayer;
        bool didDirectHit = col.TryGetComponent<Player>(out hitPlayer);

        // Apply explosion to all targets within range
        Vector3 explosionPos = transform.position;
        Collider2D[] targets = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (Collider2D target in targets)
        {
			if(target.TryGetComponent<Player>(out Player player))
			{
				Vector2 forceOffset = player.transform.position - explosionPos;
                Vector2 forceDir = forceOffset.normalized;
                float forceDist = forceOffset.magnitude;
                float forceScale = 1 - Mathf.Clamp(forceDist / explosionRadius, 0f, 1f);
                Vector2 explosionForce = explosionStrength * forceDir * forceScale;

				if(player.photonView.IsMine)
				{
					player.GetComponent<Rigidbody2D>().AddForce(explosionForce, ForceMode2D.Impulse);	
				} else player.Explode(didDirectHit, hitPlayer, transform.position, explosionRadius, minDamage, maxDamage, explosionForce);
			}
        }

        photonView.RPC("Explode", RpcTarget.All);

		// Self destruct
       	PhotonNetwork.Destroy(photonView);
    }
}
