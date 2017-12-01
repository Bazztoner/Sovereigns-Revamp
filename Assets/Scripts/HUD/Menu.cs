using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Light angelLight;
    public Light demonLight;
    public GameObject button;
    public GameObject loading;
    public void ChangeScene()
    {
        StartCoroutine(SceneLoader());
    }

    IEnumerator SceneLoader()
    {
        yield return null;

        button.SetActive(false);
        loading.SetActive(true);

        AsyncOperation ao = SceneManager.LoadSceneAsync("TransitionsTest", LoadSceneMode.Single);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            angelLight.intensity = Mathf.Lerp(angelLight.intensity, 0.2f, progress);
            demonLight.intensity = Mathf.Lerp(demonLight.intensity, 5.3f, progress);

            if (ao.progress >= 0.9f) ao.allowSceneActivation = true;

            yield return null;
        }
    }
}
