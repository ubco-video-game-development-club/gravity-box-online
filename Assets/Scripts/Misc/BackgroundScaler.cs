using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScaler : MonoBehaviour
{
    [SerializeField] private Camera target;

    void Start() 
    {
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float height = target.orthographicSize * 2.0f;
        GetComponent<SpriteRenderer>().size = new Vector2(height * aspectRatio, height);
    }
}
