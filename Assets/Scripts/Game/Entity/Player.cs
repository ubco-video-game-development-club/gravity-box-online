using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviourPun, IPunObservable
{
    public const int PLAYER_LAYER = 9;

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
    private YieldInstruction waitForEndOfFrame;

    private Vector2 realPosition;
    private float realRotation;
    private float lagDistance;
    private float angleSlerp;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        invincibilityFrameInstruction = new WaitForSeconds(invincibilityFrame / (float)numFlickers);
        waitForEndOfFrame = new WaitForEndOfFrame();
        currentHealth = maxHealth;

        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void Start()
    {
        rocketLauncher.enabled = photonView.IsMine;
        if (photonView.IsMine)
        {
            gameObject.layer = PLAYER_LAYER;
            HUD.Singleton.SetPlayer(this);
        }

        onHealthChanged.Invoke(currentHealth);
    }

    void FixedUpdate()
    {
        if (photonView.IsMine) return;
        rigidbody2D.position = Vector2.MoveTowards(rigidbody2D.position, realPosition, lagDistance * (1.0f / PhotonNetwork.SerializationRate));

        angleSlerp += 1.0f / PhotonNetwork.SerializationRate;
        rocketLauncher.transform.rotation = Quaternion.Slerp(rocketLauncher.transform.rotation, Quaternion.Euler(0, 0, realRotation), angleSlerp);
    }

    [PunRPC]
    public void TakeDamage(int damage, Vector2 explosionForce)
    {
        rigidbody2D.AddForce(explosionForce, ForceMode2D.Impulse);

        StartCoroutine(InvincibilityFrame());

        if (!photonView.IsMine) return;

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
            PhotonNetwork.Destroy(photonView);
        }

        onHealthChanged.Invoke(currentHealth);
    }

    public void Explode(bool directHit, Player hitPlayer, Vector3 position, float radius, float minDamage, float maxDamage, Vector2 explosionForce)
    {
        if (directHit && this == hitPlayer)
        {
            // Apply full damage
            photonView.RPC("TakeDamage", RpcTarget.All, Mathf.RoundToInt(maxDamage), explosionForce);
        }
        else
        {
            // Damage based on distance
            float dist = Vector2.Distance(transform.position, position);
            float damageScale = 1.0f - Mathf.Clamp(dist / radius, 0f, 1f);
            float damage = Mathf.Lerp(minDamage, maxDamage, damageScale);
            photonView.RPC("TakeDamage", RpcTarget.All, Mathf.RoundToInt(damage), explosionForce);
        }
    }

    private void OnActiveSceneChanged(Scene current, Scene next)
    {
        if (next.name != "Game") //If we're not loading the game scene,
        {
            //Destroy
            Destroy(gameObject);
        }
    }

    private IEnumerator InvincibilityFrame()
    {
        yield return waitForEndOfFrame;

        isInvincible = true;

        for (int i = 0; i < numFlickers; i++)
        {
            yield return invincibilityFrameInstruction;
            foreach (SpriteRenderer r in flickerRenderers)
            {
                r.enabled = !r.enabled;
            }
        }

        isInvincible = false;

        //This is to ensure that the renderer is always enabled at the end of this function
        //The wait time is so the flicker won't look weird
        yield return invincibilityFrameInstruction;
        foreach (SpriteRenderer r in flickerRenderers)
        {
            r.enabled = true;
        }
    }

    void OnDestroy()
    {
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length == 1)
        {
            GameOverMenu.Singleton.EndGame();
            if (players[0].photonView.IsMine) PhotonNetwork.Destroy(players[0].photonView);
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
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbody2D.position);
            stream.SendNext(rocketLauncher.transform.eulerAngles.z);
            stream.SendNext(rigidbody2D.velocity);
        }
        else
        {
            realPosition = (Vector2)stream.ReceiveNext();
            realRotation = (float)stream.ReceiveNext();
            rigidbody2D.velocity = (Vector2)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            realPosition += rigidbody2D.velocity * lag;
            lagDistance = Vector2.Distance(rigidbody2D.position, realPosition);
            angleSlerp = 0.0f;
        }
    }
}
