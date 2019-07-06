using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerColour : MonoBehaviour, IPunObservable
{
    PhotonView PV;

    void Start() {
        PV = GetComponent<PhotonView>();
    }

    public void ChangeColour(int player, Color colour) {
        if (PV.IsMine)
            PV.RPC("RPC_ChangeColour", RpcTarget.AllBuffered, player, colour.r, colour.g, colour.b);
    }

    [PunRPC] void RPC_ChangeColour(int player, float r, float g, float b) {
        if (player == 1) {
            transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(r,g,b,1);
        } else if (player == 2) {
            transform.GetChild(1).GetChild(0).GetComponent<Image>().color = new Color(r,g,b,1);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
