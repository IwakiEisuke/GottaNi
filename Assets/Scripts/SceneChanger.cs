using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AnimationClip anim;
    [SerializeField] string sceneName;

    public void ChangeScene()
    {
        animator.Play(anim.name);
        Invoke(nameof(Load), anim.length);
    }

    void Load()
    {
        SceneManager.LoadScene(sceneName);
    }
}
