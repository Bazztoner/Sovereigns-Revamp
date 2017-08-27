using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour {

    public GameObject[] ents;

	void Start ()
    {
        EventManager.AddEventListener("DoConnect", OnDoConnect);
        EventManager.AddEventListener("DoNotConnect", OnDoNotConnect);
        EventManager.AddEventListener("DividedScreen", OnDoConnect);
	}

    private void OnDoConnect(params object[] paramsContainer)
    {
        foreach (var ent in ents)
        {
            ent.SetActive(false);
        }
    }

    private void OnDoNotConnect(params object[] paramsContainer)
    {
        foreach (var ent in ents)
        {
            ent.SetActive(true);
        }
    }
}
