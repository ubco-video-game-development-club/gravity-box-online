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

    public void SetPlayer(Player player)
    {
        playerMaxHealth = player.MaxHealth;
        player.AddHealthChangedListener(OnHealthChanged);
    }
}
