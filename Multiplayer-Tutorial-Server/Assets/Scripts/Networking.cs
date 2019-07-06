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
    public GameObject Player1Text;
    public GameObject Player2Text;

    public GameObject connecting;
    public GameObject roomPanel;
    public TMP_Text roomText;
    public Button roomButton;
    public Button Disconnect;

    public Player Player1;
    [HideInInspector] public Playerinfo player1info;
    public Player Player2;
    [HideInInspector] public Playerinfo player2info;

    public GameObject PlayerPrefab;

    public ColourPicker colourPicker;

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

        if (roomText.text.Length > 1 && playerName.text.Length > 1) {
            roomButton.interactable = true;
        } else {
            roomButton.interactable = false;
        }
    }
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnPlayerEnteredRoom(Player otherPlayer) {
        base.OnPlayerEnteredRoom(otherPlayer);
        Player2Text.SetActive(true);
        Player2Text.GetComponent<TMP_Text>().text = otherPlayer.NickName;
        Player2 = otherPlayer;
        Debug.Log("test");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer == Player1) {
            Player1Text.GetComponent<TMP_Text>().text = Player2.NickName;
            Player1 = Player2;
        }

        Player2Text.SetActive(false);
        Player2 = null;
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
        connecting.SetActive(false);
        roomPanel.SetActive(true);
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
        playerPanel.SetActive(true);
        CreatePlayer();
        Disconnect.gameObject.SetActive(true);
    }

    public void CreatePlayer() {
        //if (player = player1) {
        Player[] others = PhotonNetwork.PlayerListOthers;
        if (others.Length == 0) {
            Player1Text.GetComponent<TMP_Text>().text = PhotonNetwork.NickName;
            Player1 = PhotonNetwork.LocalPlayer;
            GameObject Player1Object = Instantiate(PlayerPrefab, new Vector3(0,0,0), Quaternion.identity);
            player1info = Player1Object.GetComponent<Playerinfo>();
            player1info.playerNumber = 1;
            player1info.playerColour = colourPicker.pickedColour;
            player1info.isLocal = true;
            ChangePlayerColour(player1info.playerColour);
        } else {
            Player1 = others[0];
            Player1Text.GetComponent<TMP_Text>().text = others[0].NickName;
            Player2 = PhotonNetwork.LocalPlayer;
            Player2Text.GetComponent<TMP_Text>().text = playerName.text;
            Player2Text.SetActive(true);
            GameObject Player1Object = Instantiate(PlayerPrefab, new Vector3(0,0,0), Quaternion.identity);
            player1info = Player1Object.GetComponent<Playerinfo>();
            player1info.playerNumber = 1;
            player1info.playerColour = colourPicker.pickedColour;
            player1info.isLocal = false;
            GameObject Player2Object = Instantiate(PlayerPrefab, new Vector3(0,0,0), Quaternion.identity);
            player2info = Player2Object.GetComponent<Playerinfo>();
            player2info.playerNumber = 2;
            player2info.playerColour = colourPicker.pickedColour;
            player2info.isLocal = true;
            ChangePlayerColour(player2info.playerColour);
        }
        Player1Text.SetActive(true);
        
    }

    public void OnClickDisconnect() {
        PhotonNetwork.LeaveRoom();
        Disconnect.gameObject.SetActive(false);
    }

    public void ChangePlayerColour(Color colour) {
        if (player1info.isLocal) {
            player1info.playerColour = colour;
            playerPanel.GetComponent<PlayerColour>().ChangeColour(player1info.playerNumber, player1info.playerColour);
        } else {
            player2info.playerColour = colour;
            playerPanel.GetComponent<PlayerColour>().ChangeColour(player2info.playerNumber, player2info.playerColour);
        }
    }
}