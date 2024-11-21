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
            gameObject.SetActive(false); // ”jŠü‚³‚ê‚é‚Ü‚Å‚ÉStartƒƒ\ƒbƒh‚È‚Ç‚ªŒÄ‚Î‚ê‚È‚¢‚æ‚¤‚É‚·‚é
            Destroy(gameObject);
        }
    }
}
