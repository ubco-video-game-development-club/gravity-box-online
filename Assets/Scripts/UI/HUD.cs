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

    void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
    }
}
