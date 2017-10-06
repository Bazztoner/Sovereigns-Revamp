using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DividedScreen : MonoBehaviour {

    private Transform _cam1;
    private Transform _cam2;
    private Transform _hud1;
    private Transform _hud2;
    private Transform _player1;
    private Transform _player2;
    private GameObject _mainCam;
    private GameObject _mainHud;

	void Start ()
    {
        _cam1 = transform.Find("CameraPlayer1");
        _cam2 = transform.Find("CameraPlayer2");
        _mainCam = GameObject.Find("CameraContainer");
        _mainHud = GameObject.Find("HUD");
        _hud1 = transform.Find("HUD1");
        _hud2 = transform.Find("HUD2");
        _player1 = transform.Find("Player1");
        _player2 = transform.Find("Player2");

        EventManager.AddEventListener("DividedScreen", OnDividedScreen);
        EventManager.AddEventListener("RestartRound", OnRestartRound);
	}

    private void OnDividedScreen(params object[] paramsContainer)
    {
        _mainCam.SetActive(false);
        _mainHud.SetActive(false);
        _cam1.gameObject.SetActive(true);
        _cam2.gameObject.SetActive(true);
        _hud1.gameObject.SetActive(true);
        _hud2.gameObject.SetActive(true);
        _player1.gameObject.SetActive(true);
        _player2.gameObject.SetActive(true);

        EventManager.DispatchEvent("GameStarted");
    }

    private void OnRestartRound(params object[] paramsContainer)
    {
        if ((bool)paramsContainer[0])
        {
            _cam1.gameObject.SetActive(false);
            _cam2.gameObject.SetActive(false);
            _hud1.gameObject.SetActive(false);
            _hud2.gameObject.SetActive(false);
            _mainCam.SetActive(true);
            _mainHud.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
