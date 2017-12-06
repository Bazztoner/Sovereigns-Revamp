using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour {

    public GameObject[] ents;
    public GameObject dummiesContainer;

	void Start ()
    {
        EventManager.AddEventListener(GameEvents.DoConnect, OnDoConnect);
        EventManager.AddEventListener(GameEvents.DoNotConnect, OnDoNotConnect);
        EventManager.AddEventListener(GameEvents.DoDummyTest, OnDoDummyTest);
        EventManager.AddEventListener(GameEvents.DividedScreen, OnDoConnect);
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

    private void OnDoDummyTest(params object[] paramsContainer)
    {
        foreach (var ent in ents)
        {
            ent.SetActive(true);
        }

        dummiesContainer.SetActive(true);
    }
}
