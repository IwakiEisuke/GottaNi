using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] string background;

    void Start()
    {
        var a = SceneManager.GetSceneByName(background);
        if (a.buildIndex == -1)
        {
            SceneManager.LoadScene(background, LoadSceneMode.Additive);
        }

        //SceneManager.
        //SceneManager.LoadScene(background, LoadSceneMode.Additive);
        //SceneManager.MergeScenes()
    }
}
