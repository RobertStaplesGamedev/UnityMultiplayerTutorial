using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Net_OnLoginRequest : NetMsg
{

    public Net_OnLoginRequest() 
    {
        OP = netOP.OnLoginRequest;
    }

    public byte Success {set;get;}
    public string Information {set; get;}

    public int ConnectionId {set; get;}
    public string Token {get; set;}
    public string Username {set; get;}
    public string Discriminator {set; get;}

}

