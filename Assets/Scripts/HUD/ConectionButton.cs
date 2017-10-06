using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionButton : MonoBehaviour {
    
	void Start ()
    {
        EventManager.AddEventListener("DoConnect", Deactivate);
        EventManager.AddEventListener("DoDummyTest", Deactivate);
        EventManager.AddEventListener("DoNotConnect", Deactivate);
        EventManager.AddEventListener("DividedScreen", DeactivateOnSplit);
	}

    private void Deactivate(params object[] paramsContainer)
    {
        EventManager.RemoveEventListener("DoConnect", Deactivate);
        EventManager.RemoveEventListener("DoDummyTest", Deactivate);
        EventManager.RemoveEventListener("DoNotConnect", Deactivate);
        EventManager.RemoveEventListener("DividedScreen", DeactivateOnSplit);

        this.gameObject.SetActive(false);
    }

    private void DeactivateOnSplit(params object[] paramsContainer)
    {
        EventManager.RemoveEventListener("DoConnect", Deactivate);
        EventManager.RemoveEventListener("DoDummyTest", Deactivate);
        EventManager.RemoveEventListener("DoNotConnect", Deactivate);
        EventManager.RemoveEventListener("DividedScreen", DeactivateOnSplit);
    }
}
