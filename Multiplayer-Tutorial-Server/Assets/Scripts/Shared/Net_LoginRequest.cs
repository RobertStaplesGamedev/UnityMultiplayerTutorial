using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Net_LoginRequest : NetMsg
{

    public Net_LoginRequest() 
    {
        OP = netOP.LoginRequest;
    }

    public string UsernameOrEmail {get; set;}
    public string Password {get; set;}


}
