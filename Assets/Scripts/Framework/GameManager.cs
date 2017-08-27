using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class GameManager : Photon.MonoBehaviour {

    private bool _isGameStarted = false;

    public static bool screenDivided = false;

    public float time;

    void Start()
    {
        EventManager.AddEventListener("GameStarted", OnGameStarted);
        EventManager.AddEventListener("PlayerDeath", OnPlayerDied);
    }

    private void OnGameStarted(params object[] paramsContainer)
    {
        _isGameStarted = true;
        InvokeRepeating("DiscountTime", 1f, 1f);
        photonView.RPC("StartGame", PhotonTargets.All);
    }

    private void OnPlayerDied(params object[] paramsContainer)
    {
        var message = (string)paramsContainer[0] + " has lost the match";
        if (!PhotonNetwork.offlineMode) photonView.RPC("RpcEndGame", PhotonTargets.All, message, (string)paramsContainer[0]);
        else if (GameManager.screenDivided) EndGame((string)paramsContainer[0]);
    }

    private void DiscountTime()
    {
        time--;
        if (time == 0)
            photonView.RPC("RpcEndGame", PhotonTargets.All, "Time Out", "");
    }

    [PunRPC]
    public void StartGame()
    {
        _isGameStarted = true;
    }

    [PunRPC]
    public void RpcEndGame(string message, string player)
    {
        if (_isGameStarted)
        {
            _isGameStarted = false;
            CancelInvoke();
            print(message);
            EventManager.DispatchEvent("GameFinished", new object[] { player });
        }
    }

    public void EndGame(string player)
    {
        _isGameStarted = false;
        CancelInvoke();
        EventManager.DispatchEvent("GameFinished", new object[] { player });
    }
}
