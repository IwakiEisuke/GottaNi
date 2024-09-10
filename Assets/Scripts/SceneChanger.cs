using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip anim;
    [SerializeField] string sceneName;
    [SerializeField] bool trigger;

    public void ChangeScene()
    {
        if (animator) animator.Play(anim.name);

        if (anim)
        {
            Invoke(nameof(Load), anim.length);
        }
        else
        {
            Load();
        }
    }

    void Load()
    {
        SceneManager.LoadScene(sceneName);
    }

    private void Update()
    {
        if (trigger)
        {
            trigger = false;
            ChangeScene();
        }
    }
}
