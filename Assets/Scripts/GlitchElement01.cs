using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchElement01 : MonoBehaviour
{
    [SerializeField] float speed;
    SpriteRenderer rend;
    float X { get => rend.size.x; set => rend.size = new Vector2(value, rend.size.y); }
    float Y { get => rend.size.y; set => rend.size = new Vector2(rend.size.x, value); }

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        X += speed * Time.deltaTime;
    }

    public static GlitchElement01 Create(GlitchElement01 prefab, float speed, Vector2 localPosition, Transform parent)
    {
        var element = Instantiate(prefab, parent).GetComponent<GlitchElement01>();
        element.transform.localPosition = localPosition;
        element.speed = speed;
        return element;
    }
}
