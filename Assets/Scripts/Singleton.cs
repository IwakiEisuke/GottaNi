using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Singleton : MonoBehaviour
{
    [SerializeField] UnityEvent _activateBefore;
    static Singleton _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
            _activateBefore.Invoke();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
