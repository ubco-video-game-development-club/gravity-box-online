using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Code adapted from: http://gamedesigntheory.blogspot.com/2010/09/controlling-aspect-ratio-in-unity.html
public class CameraAspectFitter : MonoBehaviour
{
    [SerializeField] private float targetAspectRatio = 1.6f;

    private new Camera camera;

    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        // determine the game window's current aspect ratio
        float windowAspectRatio = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float heightScale = windowAspectRatio / targetAspectRatio;

        Rect scaledRect = camera.rect;
        if (heightScale < 1.0f) // add letterbox
        {
            scaledRect.width = 1.0f;
            scaledRect.height = heightScale;
            scaledRect.x = 0;
            scaledRect.y = (1.0f - heightScale) / 2.0f;
        }
        else // add pillarbox
        {
            float widthScale = 1.0f / heightScale;

            scaledRect.width = widthScale;
            scaledRect.height = 1.0f;
            scaledRect.x = (1.0f - widthScale) / 2.0f;
            scaledRect.y = 0;
        }

        camera.rect = scaledRect;
    }
}
