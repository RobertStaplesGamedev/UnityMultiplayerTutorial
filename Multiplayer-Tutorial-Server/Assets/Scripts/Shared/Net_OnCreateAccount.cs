using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Net_OnCreateAccount : NetMsg
{

    public Net_OnCreateAccount() 
    {
        OP = netOP.OnCreateAccount;
    }

    public byte Success {set;get;}
    public string Information {set; get;}


}
