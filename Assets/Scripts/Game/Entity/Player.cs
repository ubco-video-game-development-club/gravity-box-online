using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class Player : MonoBehaviourPun, IPunObservable
{
    [System.Serializable] public class OnHealthChangedEvent : UnityEvent<int> { }
    [System.Serializable] public class OnDeathEvent : UnityEvent { }

    public int MaxHealth { get { return maxHealth; } }
    [SerializeField] private int maxHealth = 5;

    [SerializeField] private float invincibilityFrame = 0.5f;
    [SerializeField] private int numFlickers = 15;
    [SerializeField] private OnHealthChangedEvent onHealthChanged = new OnHealthChangedEvent();
    [SerializeField] private OnDeathEvent onDeath = new OnDeathEvent();
    [SerializeField] private SpriteRenderer[] flickerRenderers;
	[SerializeField] private RocketLauncher rocketLauncher;

    private int currentHealth = 0;
    private bool isInvincible = false;

    private new Rigidbody2D rigidbody2D;
    private YieldInstruction invincibilityFrameInstruction;

	private Vector2 realPosition;
	private float realRotation;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        invincibilityFrameInstruction = new WaitForSeconds(invincibilityFrame / (float)numFlickers);
        currentHealth = maxHealth;
    }

	void Start()
	{
		rocketLauncher.enabled = photonView.IsMine;
	}

	void FixedUpdate()
	{
		if(photonView.IsMine) return;
		rigidbody2D.position = Vector2.Lerp(rigidbody2D.position, realPosition, Time.deltaTime);
		rocketLauncher.transform.rotation = Quaternion.Slerp(rocketLauncher.transform.rotation, Quaternion.Euler(0, 0, realRotation), Time.deltaTime);
	}

    public void TakeDamage(int damage)
    {
        //If invincible or dead, don't take damage
        if (isInvincible || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            enabled = false;
            onDeath.Invoke();
        }

        StartCoroutine(InvincibilityFrame());

        onHealthChanged.Invoke(currentHealth);
    }

    private IEnumerator InvincibilityFrame()
    {
        isInvincible = true;

        for(int i = 0; i < numFlickers; i++)
        {
            yield return invincibilityFrameInstruction;
            foreach(SpriteRenderer r in flickerRenderers)
            {
                r.enabled = !r.enabled;
            }
        }

        isInvincible = false;

        //This is to ensure that the renderer is always enabled at the end of this function
        //The wait time is so the flicker won't look weird
        yield return invincibilityFrameInstruction;
        foreach(SpriteRenderer r in flickerRenderers)
        {
                r.enabled = true;
        }
    }

    public void ApplyKnockback(Vector2 knockbackForce)
    {
        rigidbody2D.AddForce(knockbackForce);
    }

    public void AddHealthChangedListener(UnityAction<int> call) 
    {
        onHealthChanged.AddListener(call);
    }

    public void RemoveHealthChangedListener(UnityAction<int> call) 
    {
        onHealthChanged.RemoveListener(call);
    }

    public void AddDeathListener(UnityAction call) 
    {
        onDeath.AddListener(call);
    }

    public void RemoveDeathListener(UnityAction call) 
    {
        onDeath.RemoveListener(call);
    }

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if(stream.IsWriting)
		{
			stream.SendNext(rigidbody2D.position);
			stream.SendNext(rocketLauncher.rotation.z);
		} else 
		{
			realPosition = (Vector2)stream.ReceiveNext();
			realRotation = (float)stream.ReceiveNext();
		}
	}
}
