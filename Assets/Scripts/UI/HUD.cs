using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static HUD Singleton { get; private set; }

    public RectTransform HealthBarParent { get { return healthBarParent; } }
    [SerializeField] private RectTransform healthBarParent;

    public RectTransform ExplosionTextParent { get { return explosionTextParent; } }
    [SerializeField] private RectTransform explosionTextParent;

    public Slider HPSlider { get { return hpSlider; } }
    [SerializeField] private Slider hpSlider;

    [SerializeField] private GameObject gravityWellDisplay;
    [SerializeField] private RectTransform gravityWellCooldownOverlay;
    [SerializeField] private ParticleSystem gravityWellActiveEffect;

    private int playerMaxHealth;

    void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }

    public void OnHealthChanged(int value)
    {
        float percent = (float)value / (float)playerMaxHealth;
        hpSlider.value = percent;
    }

    public void OnPlayerDie()
    {
        hpSlider.gameObject.SetActive(false);
        gravityWellDisplay.SetActive(false);
    }

    public void SetPlayer(Player player)
    {
        playerMaxHealth = player.MaxHealth;
        hpSlider.gameObject.SetActive(true);

        gravityWellDisplay.SetActive(true);
        gravityWellCooldownOverlay.anchorMax = new Vector2(1f, 0);
        gravityWellActiveEffect.Play();

        player.AddHealthChangedListener(OnHealthChanged);
        player.AddDeathListener(OnPlayerDie);
    }

    public void StartGravityWellCooldown(float cooldown)
    {
        StartCoroutine(GravityWellCooldown(cooldown));
    }

    private IEnumerator GravityWellCooldown(float cooldown)
    {
        gravityWellActiveEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        float progress = 0;
        while (progress < cooldown)
        {
            gravityWellCooldownOverlay.anchorMax = new Vector2(1f, 1 - progress / cooldown);
            progress += Time.deltaTime;
            yield return null;
        }
        gravityWellCooldownOverlay.anchorMax = new Vector2(1f, 0);
        gravityWellActiveEffect.Play();
    }
}
