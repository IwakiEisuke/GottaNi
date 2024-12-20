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
            gameObject.SetActive(false); // 破棄されるまでにStartメソッドなどが呼ばれないようにする
            Destroy(gameObject);
        }
    }
}
