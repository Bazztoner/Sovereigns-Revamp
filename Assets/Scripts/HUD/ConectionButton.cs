using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionButton : MonoBehaviour {
    
	void Start ()
    {
        EventManager.AddEventListener("DoConnect", Deactivate);
        EventManager.AddEventListener("DoNotConnect", Deactivate);
        EventManager.AddEventListener("DividedScreen", Deactivate);
	}

    private void Deactivate(params object[] paramsContainer)
    {
        this.gameObject.SetActive(false);
    }
}
