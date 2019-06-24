using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

public class Networking : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public string versionName= "0.1";

    public Button BtnConnectRoom;

    public bool TriesToConnectToMaster;
    public bool TriesToConnectToRoom;

    public TMP_Text RoomName;
    public TMP_Text playerName;

    public GameObject playerPanel;
    public TMP_Text Player1;
    public TMP_Text Player2;
    int PlayerCount = 0;

void Start()
    {
        TriesToConnectToMaster = false;
        TriesToConnectToRoom   = false;

        DontDestroyOnLoad(this);
        ConnectToMaster();
    }

    // Update is called once per frame
    void Update()
    {
        BtnConnectRoom.gameObject.SetActive(PhotonNetwork.IsConnected && !TriesToConnectToMaster && !TriesToConnectToRoom);


    }
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer) {
        base.OnPlayerEnteredRoom(otherPlayer);
        if (!otherPlayer.IsInactive) {
            Player2.text = otherPlayer.NickName;
        }
    }

    public void ConnectToMaster()
    {
        //Settings (all optional and only for tutorial purpose)
        PhotonNetwork.OfflineMode = false;           //true would "fake" an online connection
        PhotonNetwork.GameVersion = "v1";            //only people with the same game version can play together
        TriesToConnectToMaster = true;
        //PhotonNetwork.ConnectToMaster(ip,port,appid); //manual connection
        PhotonNetwork.ConnectUsingSettings();           //automatic connection based on the config file in Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        TriesToConnectToMaster = false;
        Debug.Log("Connected to Master!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        TriesToConnectToMaster = false;
        TriesToConnectToRoom   = false;
        Debug.Log(cause);
    }

    public void OnClickConnectToRoom()
    {
        PhotonNetwork.NickName = playerName.text;       //to set a player name
        if (!PhotonNetwork.IsConnected)
            return;

        TriesToConnectToRoom = true;
        PhotonNetwork.JoinLobby();
        
        //PhotonNetwork.JoinRoom(RoomName.text);   //Join a specific Room   - Error: OnJoinRoomFailed  
        //PhotonNetwork.JoinRandomRoom();               //Join a random Room     - Error: OnJoinRandomRoomFailed  
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom(RoomName.text, new RoomOptions { MaxPlayers = 2 }, null); //Create a specific Room - Error: OnCreateRoomFailed
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        //no room available
        //create a room (null as a name means "does not matter")
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        base.OnCreateRoomFailed(returnCode, message);
        TriesToConnectToRoom = false;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        TriesToConnectToRoom = false;
        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name);
        PlaceName();
    }

    public void PlaceName() {
        //if (player = player1) {
        Player[] others = PhotonNetwork.PlayerListOthers;
        if (others.Length == 0) {
            Player1.text = PhotonNetwork.NickName;
        } else {
            Debug.Log(others[0].NickName);
            Player1.text = others[0].NickName;
            Player2.text = playerName.text;
        }
        PlayerCount++;
    }
}