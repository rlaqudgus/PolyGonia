using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SceneLoading : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loaindgBar;

    public void LoadScene(int sceneId)
    {
        StartCoroutine(LoadSceneAsync(sceneId));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneID);
        
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            float curProgress = Mathf.Clamp01(operation.progress / 0.9f);

            loaindgBar.value = curProgress;

            yield return null;
        }
    }
}
