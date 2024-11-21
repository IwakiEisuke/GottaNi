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
            gameObject.SetActive(false); // �j�������܂ł�Start���\�b�h�Ȃǂ��Ă΂�Ȃ��悤�ɂ���
            Destroy(gameObject);
        }
    }
}
