using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Net_CreateAccount : NetMsg
{

    public Net_CreateAccount() 
    {
        OP = netOP.CreateAccount;
    }

    public string username {get; set;}
    public string password {get; set;}
    public string Email {get;set;}


}
