using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PhotonView))]
public class GameManager : Photon.MonoBehaviour
{

    private bool _isGameStarted = false;
    private int _round;
    private int _maxRound = 3;
    private int _player1Score = 0;
    private int _player2Score = 0;
    private float _maxTime;
    private float _restartTime = 3f;
    private PlayerStats _p1;
    private PlayerStats _p2;

    public static bool screenDivided = false;

    public float time;

    void Start()
    {
        _maxTime = time;
        EventManager.AddEventListener("GameStarted", OnGameStarted);
        EventManager.AddEventListener("PlayerDeath", OnPlayerDied);
    }

    private void OnGameStarted(params object[] paramsContainer)
    {
        _isGameStarted = true;
        _round = 0;
        _player1Score = 0;
        _player2Score = 0;
        time = _maxTime;
        InvokeRepeating("DiscountTime", 1f, 1f);
        if (!GameManager.screenDivided) photonView.RPC("StartGame", PhotonTargets.All);
        else
        {
            if (_p1 == null) _p1 = GameObject.Find("Player1").GetComponent<PlayerStats>();
            if (_p2 == null) _p2 = GameObject.Find("Player2").GetComponent<PlayerStats>();
        }
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
        {
            if (GameManager.screenDivided)
                EndGame("");
            else
                photonView.RPC("RpcEndGame", PhotonTargets.All, "Time Out", "");
        }
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

        if (player == "Player1") _player1Score++;
        else if (player == "Player2") _player2Score++;
        else if (player == "")
        {
            if (_p1.Hp > _p2.Hp) _player1Score++;
            else if (_p1.Hp < _p2.Hp) _player2Score++;
            else
            {
                _player1Score++;
                _player2Score++;
            }
        }

        Invoke("NextRound", _restartTime);
        EventManager.DispatchEvent("GameFinished", new object[] { player });
    }

    private void NextRound()
    {
        _round++;

        if (_round >= _maxRound || ((_player1Score >= (_maxRound / 2) + 1) || (_player2Score >= (_maxRound / 2) + 1)))
            EndMatch();
        else
            RestartRound();
    }

    private void RestartRound()
    {
        EventManager.DispatchEvent("RestartRound", new object[] { false });
        time = _maxTime;
        _isGameStarted = true;
        InvokeRepeating("DiscountTime", 1f, 1f);
    }

    private void EndMatch()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DestructibleObject.DeleteAllObjs();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync("Loading");
        //EventManager.DispatchEvent("RestartRound", new object[] { true });
    }
}
