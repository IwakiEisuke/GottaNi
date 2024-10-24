using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureConverter : MonoBehaviour
{
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Image image;

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

        // Texture2DからSpriteを作成
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        if (sr) sr.sprite = sprite;
        if (image) image.sprite = sprite;
    }
}
