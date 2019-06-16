using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class netOP
{
    public const int None = 0;
    public const int CreateAccount = 1;
    public const int OnCreateAccount = 2;
    public const int LoginRequest = 3;
    public const int OnLoginRequest = 4;
}

[System.Serializable]
public abstract class NetMsg
{
    public byte OP {set; get;}

    public NetMsg()
    {
        OP = netOP.None;
    }
}
