using System.Collections.Generic;
using UnityEngine;

public class BackGround01 : MonoBehaviour
{
    [SerializeField] SpriteRenderer backGround;
    [SerializeField] GlitchElement01 element;
    [SerializeField] int count;
    [SerializeField] float maxSpeed, minSpeed;
    [SerializeField] float spacing;
    [SerializeField] float height;

    List<GlitchElement01> elements = new();

    private void Start()
    {
        for (int i = 0; i < count; i++)
        {
            var speed = Random.Range(minSpeed, maxSpeed);
            speed = speed > 0 ? Mathf.Max(speed, spacing) : Mathf.Min(speed, -spacing);

            var c = Random.Range(0, 1f) > 0.5f ? 10 : 1;

            for (int j = 0; j < c; j++)
            {
                var renderer = GlitchElement01.Create(element, speed, new Vector2(j * 0.015f, i * height), transform);
                elements.Add(renderer);
            }
        }
    }
}