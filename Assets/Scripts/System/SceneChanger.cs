using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] string sceneName;
    [SerializeField] string[] unloadScenes;
    [SerializeField] bool trigger;
    [SerializeField] float additionalDuration;

    public void ChangeScene()
    {
        if (director)
        {
            director.Play();
            Invoke(nameof(Load), (float)director.duration + additionalDuration);
        }
        else
        {
            Invoke(nameof(Load), additionalDuration);
        }
    }

    void Load()
    {
        StartCoroutine(nameof(LoadScene), sceneName);
    }

    public void UnloadScene(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }

    private void Update()
    {
        if (trigger)
        {
            trigger = false;
            ChangeScene();
        }
    }

    public IEnumerator LoadScene(string scene)
    {
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

        var loadScene = SceneManager.GetSceneByName(scene);
        SceneManager.SetActiveScene(loadScene);

        foreach (string unload in unloadScenes)
        {
            if (SceneManager.GetSceneByName(unload).isLoaded)
            {
                SceneManager.UnloadSceneAsync(unload);
            }
        }
    }
}
