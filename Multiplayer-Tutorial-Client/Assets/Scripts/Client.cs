using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    public static Client Instance {private set; get;}

    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE= 1024;

    public string SERVER_IP = "127.0.0.1";
    private byte error;

    private byte reliableChannel;
    private int connectionId;
    private int hostID;

    private bool isStarted = false;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Update() {
        UpdateMessagePump();
    }
    public void Init() {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        
        HostTopology topo = new HostTopology(cc, MAX_USER);

        //Client only code
        hostID = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        //webclient
        connectionId = NetworkTransport.Connect(hostID, SERVER_IP, WEB_PORT, 0, out error);
        Debug.Log("Connecting from web");
#else
        //standaloneclient
        connectionId = NetworkTransport.Connect(hostID, SERVER_IP, PORT, 0, out error);
        Debug.Log("Connecting from standalone");
#endif
        Debug.Log(string.Format("Attpemting connection on {0}...", SERVER_IP));
        isStarted = true;
    }
    public void Shutdown() {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump() {
        if (!isStarted)
            return;

        int recHostId;
        int connectionId;
        int channelid;

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelid, recBuffer, recBuffer.Length, out dataSize, out error);
        switch (type) {
            case NetworkEventType.Nothing :
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log(string.Format("Connected to server on {0} ip", SERVER_IP));
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("disconnected from server on {0} ip", SERVER_IP));
                break;
            case NetworkEventType.DataEvent:
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(recBuffer);
                NetMsg msg = (NetMsg)formatter.Deserialize(ms);

                OnData(connectionId, channelid, recHostId, msg);
                break;

            default:
            case NetworkEventType.BroadcastEvent:
                Debug.Log("unexpected network event type");
                break;
        }
    }

    #region OnData
    private void OnData(int cnnid, int channelid, int recHostId, NetMsg msg) {
        switch (msg.OP)
        {
            case netOP.None:
                Debug.Log("unexpected NETOP");
                break;
            case netOP.OnCreateAccount:
                OnCreateAccount((Net_OnCreateAccount)msg);
                break;
            case netOP.OnLoginRequest:
                OnLoginRequest((Net_OnLoginRequest)msg);
                break;
        }
    }

    private void OnCreateAccount(Net_OnCreateAccount oca) {
        LobbyScene.Instance.EnableInputs(true);
        LobbyScene.Instance.SetAuthMessage(oca.Information);
    }
    private void OnLoginRequest(Net_OnLoginRequest olr) {
        if (olr.Success != 0) {
            LobbyScene.Instance.EnableInputs(true);
        } else {
            Debug.Log("SuccesfulLogin");
        }
    }
    #endregion

    #region Send
    public void SendServer(NetMsg msg) {
        byte[] buffer = new byte[BYTE_SIZE];

        //this is where you would crush your data
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);

        NetworkTransport.Send(hostID, connectionId, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    public void SendCreateAccount(string username, string password, string email) {
        Net_CreateAccount ca = new Net_CreateAccount();

        ca.Email = email;
        ca.username = username;
        ca.password = password;

        SendServer(ca);
    }
    public void SendLoginRequest(string username, string password) {
        Net_LoginRequest lr = new Net_LoginRequest();

        lr.UsernameOrEmail = username;
        lr.Password = password;

        SendServer(lr);
    }
    #endregion
}
