using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour {

    private SpawnSpot[] _spawnSpots;

    public bool offlineMode = true;

	void Start ()
    {
        _spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        EventManager.AddEventListener("DoConnect", OnDoConnect);
        EventManager.AddEventListener("DoNotConnect", OnDoNotConnect);
        EventManager.AddEventListener("DividedScreen", OnDoNotConnect);
    }

    private void OnDoConnect(params object[] paramsContainer)
    {
        offlineMode = false;
        Connect();
    }

    private void OnDoNotConnect(params object[] paramsContainer)
    {
        offlineMode = true;
        Connect();
    }

    public void Connect()
    {
        if (!offlineMode)
        {
            PhotonNetwork.player.NickName = "Player " + Random.Range(0, 100);
            PhotonNetwork.ConnectUsingSettings("ProjectSeminarV002");
        }
        else PhotonNetwork.offlineMode = true;
    }

    private void OnJoinedLobby()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsVisible = false;
        ro.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("SeminarRoom", ro, TypedLobby.Default);
        print("OnJoinedLobby");
    }

    private void OnPhotonJoinRoomFailed()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsVisible = false;
        ro.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom("SeminarRoom", ro, TypedLobby.Default);
        print("OnPhotonJoinRoomFailed");
        //Esto ya no va a hacer falta cuando quite el random.
    }

    private void OnJoinedRoom()
    {
        if (PhotonNetwork.room.PlayerCount == 2) SpawnPlayer();

        //SpawnPlayer();
        print("OnJoinedRoom");
    }

    private void OnPhotonPlayerConnected()
    {
        print("OnPhotonPlayerConnected");
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            SpawnPlayer();
            EventManager.DispatchEvent("GameStarted");
        } 
    }

    private void SpawnPlayer()
    {
        SpawnSpot spot = _spawnSpots[Random.Range(0, _spawnSpots.Length)]; //TODO, cambiar para que spawnee en un lugar especifico dependiendo jugador.
        GameObject myCharacter = PhotonNetwork.Instantiate("Player1", spot.transform.position, spot.transform.rotation, 0);
        
        myCharacter.GetComponent<Player1Input>().enabled = true;
        myCharacter.GetComponent<PlayerMovement>().enabled = true;
        myCharacter.GetComponent<PlayerStats>().enabled = true;
        myCharacter.GetComponent<PlayerCombat>().enabled = true;
        myCharacter.GetComponent<PlayerSkills>().enabled = true;
        myCharacter.GetComponent<PlayerParticles>().enabled = true;
        myCharacter.GetComponent<AnimationController>().enabled = true;
        //CAMBIO: ESTABA EN FALSE, LO PASAMOS A TRUE
        myCharacter.GetComponent<NetworkAttacks>().enabled = true;
        myCharacter.GetComponentInChildren<SwordScript>().enabled = true;
        myCharacter.GetComponent<Rigidbody>().isKinematic = false;
        myCharacter.GetComponent<PlayerStats>().Initialize();
        myCharacter.GetComponent<Animator>().applyRootMotion = true;
    }

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}
