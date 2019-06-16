using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 26000;
    private const int WEB_PORT = 26001;
    private const int BYTE_SIZE= 1024;

    private byte reliableChannel;
    private int hostID;
    private int webHostID;

    private bool isStarted = false;

    private byte error;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    void Update()
    {
        UpdateMessagePump();
    }
    public void Init() {
        NetworkTransport.Init();

        ConnectionConfig cc = new ConnectionConfig();
        reliableChannel = cc.AddChannel(QosType.Reliable);
        
        HostTopology topo = new HostTopology(cc, MAX_USER);

        //Serveronly code
        hostID = NetworkTransport.AddHost(topo, PORT, null);
        webHostID = NetworkTransport.AddWebsocketHost(topo, WEB_PORT, null);


        Debug.Log(string.Format("Opening connection on port {0} and websocker{1}", PORT, WEB_PORT));
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
                Debug.Log(string.Format("User {0} has connected through host {1}", connectionId, recHostId));
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log(string.Format("User {0} has disconnected!", connectionId));
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
            case netOP.CreateAccount:
                CreateAccount(cnnid, channelid, recHostId, (Net_CreateAccount)msg);
                break;
            case netOP.LoginRequest:
                LoginRequest(cnnid, channelid, recHostId, (Net_LoginRequest)msg);
                break;
        }
    }


    private void CreateAccount(int cnnid, int channelid, int recHostId, Net_CreateAccount msg) {
        Debug.Log(string.Format("{0} {1} {2}", msg.username, msg.password, msg.Email));

        Net_OnCreateAccount oca = new Net_OnCreateAccount();
        oca.Success = 0;
        oca.Information = "Account Created";

        SendClient(recHostId, cnnid, oca);
    }
    private void LoginRequest(int cnnid, int channelid, int recHostId, Net_LoginRequest lr) {
        Debug.Log(string.Format("{0} {1}", lr.UsernameOrEmail, lr.Password));
        Net_OnLoginRequest olr = new Net_OnLoginRequest();
        olr.Success = 0;
        olr.Information = "User Logged in";
        olr.Token = "Token";
        olr.Username = "rob staples";
        olr.Discriminator = "#0001";

        SendClient(recHostId, cnnid, olr);
    }
    #endregion

    #region Send
    public void SendClient(int recHost, int cnnid, NetMsg msg) {
        byte[] buffer = new byte[BYTE_SIZE];

        //this is where you would crush your data
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, msg);


        if (recHost == 0)
            NetworkTransport.Send(hostID, cnnid, reliableChannel, buffer, BYTE_SIZE, out error);
        else
            NetworkTransport.Send(webHostID, cnnid, reliableChannel, buffer, BYTE_SIZE, out error);
    }
    #endregion
}
