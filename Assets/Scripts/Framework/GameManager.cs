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
        EventManager.AddEventListener(GameEvents.GameStarted, OnGameStarted);
        EventManager.AddEventListener(CharacterEvents.PlayerDeath, OnPlayerDied);
    }

    private void OnGameStarted(params object[] paramsContainer)
    {
        _isGameStarted = true;
        _round = 0;
        _player1Score = 0;
        _player2Score = 0;
        time = _maxTime;
        if (!GameManager.screenDivided) photonView.RPC("StartGame", PhotonTargets.All);
        else
        {
            if (_p1 == null) _p1 = GameObject.Find("Player1").GetComponent<PlayerStats>();
            if (_p2 == null) _p2 = GameObject.Find("Player2").GetComponent<PlayerStats>();
        }
    }

    private void Update()
    {
        if (_isGameStarted && Input.GetKeyDown(KeyCode.P))
        {
            if (_p1 == null) _p1 = GameObject.Find("Player1").GetComponent<PlayerStats>();
            if (_p2 == null) _p2 = GameObject.Find("Player2").GetComponent<PlayerStats>();
            _p1.RegainMana(100);
            _p2.RegainMana(100);
        }
    }

    private void OnPlayerDied(params object[] paramsContainer)
    {
        //var message = (string)paramsContainer[0] + " has lost the match";
        if (GameManager.screenDivided) EndGame((string)paramsContainer[0]);
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
            EventManager.DispatchEvent(GameEvents.GameFinished, new object[] { player });
        }
    }

    public void EndGame(string player)
    {
        _isGameStarted = false;
        CancelInvoke();

        if (player == "Player1") _player2Score++;
        else if (player == "Player2") _player1Score++;
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

        NextRound(player);
    }

    private void NextRound(string player)
    {
        _round++;

        if (_round >= _maxRound || ((_player1Score >= (_maxRound / 2) + 1) || (_player2Score >= (_maxRound / 2) + 1)))
        {
            EventManager.DispatchEvent(GameEvents.EndOfMatch, new object[] { _player1Score, _player2Score });
            Invoke("EndMatch", _restartTime);
        }
        else
        {
            EventManager.DispatchEvent(GameEvents.GameFinished, new object[] { player });
            Invoke("RestartRound", _restartTime);
        }
    }

    private void RestartRound()
    {
        EventManager.DispatchEvent(GameEvents.RestartRound, new object[] { false });
        EventManager.DispatchEvent(UIEvents.SetRoundText, new object[] { _round });
        time = _maxTime;
        _isGameStarted = true;
    }

    private void EndMatch()
    {
        EventManager.DispatchEvent(GameEvents.EndMatch);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        DestructibleObject.DeleteAllObjs();
        ParticleManager.DestroyInstance();
        SlowMotion.DestroyInstance();
        EventManager.ClearAllEvents();

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync("Loading");
    }
}
