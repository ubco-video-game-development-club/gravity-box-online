using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [System.Serializable] public class OnHealthChangedEvent : UnityEvent<float, float> { }
    [System.Serializable] public class OnDeathEvent : UnityEvent { }

    public float Health { get { return currentHealth; } }

    [SerializeField] private float maxHealth = 10.0f;
    [SerializeField] private HealthBar healthBarPrefab;
    [SerializeField] private float healthBarYOffset = -0.5f;
    [SerializeField] private int pointsValue = 75;
    [SerializeField] private OnHealthChangedEvent onHealthChanged = new OnHealthChangedEvent();
    [SerializeField] private OnDeathEvent onDeath = new OnDeathEvent();

    private float currentHealth;
    private HealthBar healthBar;

    protected void Awake()
    {
        currentHealth = maxHealth;

        // Initialize health bar
        Vector3 healthBarSpawn = transform.position + Vector3.up * healthBarYOffset;
        healthBar = Instantiate(healthBarPrefab, healthBarSpawn, Quaternion.identity, HUD.Singleton.HealthBarParent);
        AddHealthChangedListener(healthBar.UpdateHealth);
    }

    protected void FixedUpdate()
    {
        healthBar.transform.position = transform.position + Vector3.up * healthBarYOffset;
    }

    public void TakeDamage(float damage)
    {
        if(damage < 0f) return; //Can't to negative damage
        currentHealth -= damage;
        onHealthChanged.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0.0f)
        {
            GameManager.ScoreSystem.AddScore(pointsValue);
            onDeath.Invoke();
            Destroy(healthBar.gameObject);
            Destroy(gameObject);
        }
    }

    public void AddHealthChangedListener(UnityAction<float, float> listener)
    {
        onHealthChanged.AddListener(listener);
    }

    public void RemoveHealthChangedListener(UnityAction<float, float> listener)
    {
        onHealthChanged.RemoveListener(listener);
    }

    public void AddDeathListener(UnityAction listener)
    {
        onDeath.AddListener(listener);
    }

    public void RemoveDeathListener(UnityAction listener)
    {
        onDeath.RemoveListener(listener);
    }
}
