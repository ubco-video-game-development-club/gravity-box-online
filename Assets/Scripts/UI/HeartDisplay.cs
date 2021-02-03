using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartDisplay : MonoBehaviour
{
    [SerializeField] private Image heartPrefab;
    [SerializeField] private Sprite filledHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private float horizontalGap = 5;

    private List<Image> hearts;

    void Awake()
    {
        hearts = new List<Image>();
    }

    void Start()
    {
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.AddHealthChangedListener(UpdateHeartDisplay);

        float spawnX = 0;
        float xWidth = heartPrefab.rectTransform.rect.width;
        for (int i = 0; i < player.MaxHealth; i++)
        {
            Image heart = Instantiate(heartPrefab, transform);
            heart.rectTransform.anchoredPosition += new Vector2(spawnX, 0);
            spawnX -= xWidth + horizontalGap;
            hearts.Add(heart);
        }
        
        UpdateHeartDisplay(player.MaxHealth);
    }

    private void UpdateHeartDisplay(int health)
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].sprite = i < health ? filledHeart : emptyHeart;
        }
    }
}
