using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionButton : MonoBehaviour {
    
	void Start ()
    {
        EventManager.AddEventListener(GameEvents.DoConnect, Deactivate);
        EventManager.AddEventListener(GameEvents.DoDummyTest, Deactivate);
        EventManager.AddEventListener(GameEvents.DoNotConnect, Deactivate);
        EventManager.AddEventListener(GameEvents.DividedScreen, DeactivateOnSplit);
	}

    private void Deactivate(params object[] paramsContainer)
    {
        EventManager.RemoveEventListener(GameEvents.DoConnect, Deactivate);
        EventManager.RemoveEventListener(GameEvents.DoDummyTest, Deactivate);
        EventManager.RemoveEventListener(GameEvents.DoNotConnect, Deactivate);
        EventManager.RemoveEventListener(GameEvents.DividedScreen, DeactivateOnSplit);

        this.gameObject.SetActive(false);
    }

    private void DeactivateOnSplit(params object[] paramsContainer)
    {
        EventManager.RemoveEventListener(GameEvents.DoConnect, Deactivate);
        EventManager.RemoveEventListener(GameEvents.DoDummyTest, Deactivate);
        EventManager.RemoveEventListener(GameEvents.DoNotConnect, Deactivate);
        EventManager.RemoveEventListener(GameEvents.DividedScreen, DeactivateOnSplit);
    }
}
