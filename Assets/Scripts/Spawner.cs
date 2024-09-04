using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject go;

    public void Spawn()
    {
        Instantiate(go, transform);
    }
}
