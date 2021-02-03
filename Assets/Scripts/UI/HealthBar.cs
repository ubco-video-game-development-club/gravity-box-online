using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private RectTransform fillImage;

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        fillImage.anchorMax = new Vector2(currentHealth / maxHealth, 1);
    }
}
