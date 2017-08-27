using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividedScreen : MonoBehaviour {
    
	void Start ()
    {
        EventManager.AddEventListener("DividedScreen", OnDividedScreen);
	}

    private void OnDividedScreen(params object[] paramsContainer)
    {
        var cam1 = transform.Find("CameraPlayer1");
        var cam2 = transform.Find("CameraPlayer2");
        var mainCam = GameObject.Find("CameraContainer");
        var mainHud = GameObject.Find("HUD");
        var hud1 = transform.Find("HUD1");
        var hud2 = transform.Find("HUD2");
        var player1 = transform.Find("Player1");
        var player2 = transform.Find("Player2");

        mainCam.SetActive(false);
        mainHud.SetActive(false);
        cam1.gameObject.SetActive(true);
        cam2.gameObject.SetActive(true);
        hud1.gameObject.SetActive(true);
        hud2.gameObject.SetActive(true);
        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);
    }
}
