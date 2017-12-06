using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public static SlowMotion instance;
    public float waitTime;
    public float scale;

    void Start()
    {
        EventManager.AddEventListener(CharacterEvents.CharacterDamaged, Activate);
    }

    public static void DestroyInstance()
    {
        instance = null;
    }

    public void Activate(params object[] info)
    {
        StartCoroutine(SlowMo(waitTime, scale));
    }

    IEnumerator SlowMo(float waitTime, float scale)
    {
        Time.timeScale = scale;

        var i = 0f;

        while (i <= 1)
        {
            i += Time.unscaledDeltaTime / waitTime * 0.8f;
            Time.timeScale = Mathf.Lerp(Time.timeScale, scale, i);
            yield return new WaitForEndOfFrame();
        }

        i = 0;

        while (i <= 1)
        {
            i += Time.unscaledDeltaTime / waitTime * 0.2f;
            Time.timeScale = Mathf.Lerp(scale, 1, i);
            yield return new WaitForEndOfFrame();
        }
    }
}
