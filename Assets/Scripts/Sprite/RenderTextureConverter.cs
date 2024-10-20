using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureConverter : MonoBehaviour
{
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] SpriteRenderer sr;

    Texture2D texture;

    void Start()
    {
        texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
    }

    void Update()
    {
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // Texture2DÇ©ÇÁSpriteÇçÏê¨
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        sr.sprite = sprite;
    }
}
