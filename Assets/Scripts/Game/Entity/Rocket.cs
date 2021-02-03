using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject explosionEffectPrefab;
    [SerializeField] private GameObject explosionTextPrefab;
    [SerializeField] private float speed = 1;
    [SerializeField] private float explosionStrength = 1;
    [SerializeField] private float explosionRadius = 1;
    [SerializeField] private float minDamage = 3.0f;
    [SerializeField] private float maxDamage = 10.0f;

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

    private void ApplyVelocity(Vector2 velocity)
    {
        transform.rotation = Quaternion.FromToRotation(Vector2.right, velocity);
        rigidbody2D.velocity = velocity;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Get enemy hit if we did direct hit
        Enemy hitEnemy;
        bool didDirectHit = col.TryGetComponent<Enemy>(out hitEnemy);

        // Apply explosion to all targets within range
        Vector3 explosionPos = transform.position;
        Collider2D[] targets = Physics2D.OverlapCircleAll(explosionPos, explosionRadius);
        foreach (Collider2D target in targets)
        {
            if (target.TryGetComponent<Rigidbody2D>(out Rigidbody2D targetBody))
            {
                Vector2 forceOffset = targetBody.transform.position - explosionPos;
                Vector2 forceDir = forceOffset.normalized;
                float forceDist = forceOffset.magnitude;
                float forceScale = 1 - Mathf.Clamp(forceDist / explosionRadius, 0f, 1f);
                targetBody.AddForce(explosionStrength * forceDir * forceScale, ForceMode2D.Impulse);
            }

            if (target.TryGetComponent<Enemy>(out Enemy enemy))
            {
                // Check if we direct hit this enemy
                if (didDirectHit && enemy == hitEnemy)
                {
                    // Apply full damage
                    enemy.TakeDamage(maxDamage);
                }
                else
                {
                    // Damage based on distance
                    float enemyDist = Vector2.Distance(enemy.transform.position, transform.position);
                    float damageScale = 1 - Mathf.Clamp(enemyDist / explosionRadius, 0f, 1f);
                    float damage = Mathf.Lerp(minDamage, maxDamage, damageScale);
                    enemy.TakeDamage(damage);
                }
            }
        }

        // Spawn explosion effect
        Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Instantiate(explosionTextPrefab, transform.position, Quaternion.identity, HUD.Singleton.ExplosionTextParent);

        // Self destruct
        Destroy(gameObject);
    }
}
