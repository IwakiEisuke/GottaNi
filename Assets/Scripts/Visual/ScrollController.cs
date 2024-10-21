using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] float scrollSpeedX;
    [SerializeField] float scrollSpeedY;
    Material m;

    private void Start()
    {
        ResetMaterial();

        if (m)
        {
            m.SetFloat("_IsPlaying", 1);
            m.SetVector("_ScrollSpeed", new Vector2(scrollSpeedX, scrollSpeedY));
        }
    }

    private void ResetMaterial()
    {
        if (!image)
        {
            image = GetComponent<Image>();
        }

        if (image && !m)
        {
            m = new(image.material);
            m.name = image.material.name + " (Instance)";
            m.SetFloat("_IsPlaying", 0);
            m.SetVector("_ScrollSpeed", new Vector2(0, 0));
            image.material = m;
        }
    }

    private void OnDestroy()
    {
        if (m)
        {
            Destroy(m);
        }
    }
}
