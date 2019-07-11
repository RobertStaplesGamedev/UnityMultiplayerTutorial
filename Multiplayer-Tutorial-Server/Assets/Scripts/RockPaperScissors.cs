using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class RockPaperScissors : MonoBehaviour, IPunObservable
{
    PhotonView PV;

    public bool player1IsLocked = false;
    public int player1Selection;
    public bool player2IsLocked = false;
    public int player2Selection;

    public TMP_Text winnerText;

    public Playerinfo Player;

    public Button scissors;
    public Button paper;
    public Button rock;

    //0 is scissors
    //1 is Paper
    //2 is Rock

    void Start() {
        PV = GetComponent<PhotonView>();
    }

    public void OnClickSelectOption(int selection) {
        if (Player.playerNumber == 1) {
            player1Selection = selection;
        } else if (Player.playerNumber == 2) {
            player2Selection = selection;
        }
        if (selection == 0) {
            scissors.interactable = false;
            paper.interactable = true;
            rock.interactable = true;
        } else if (selection == 1) {
            scissors.interactable = true;
            paper.interactable = false;
            rock.interactable = true;
        } else if (selection == 2) {
            scissors.interactable = true;
            paper.interactable = true;
            rock.interactable = false;
        }
    }

    public void OnClickLockSelection () {
        if (Player.playerNumber == 1) {
            PV.RPC("RPC_LockSelection", RpcTarget.AllBuffered, Player.playerNumber, player1Selection, true);
        }
        else if (Player.playerNumber == 2) {
            PV.RPC("RPC_LockSelection", RpcTarget.AllBuffered, Player.playerNumber, player2Selection, true);
        }
    }

    [PunRPC] void RPC_LockSelection(int player, int selection, bool isLocked) {
        if (player == 1) {
            player1Selection = selection;
            player1IsLocked = isLocked;
        } else if (player == 2) {
            player2Selection = selection;
            player2IsLocked = isLocked;
        }
        if (player1IsLocked && player2IsLocked) {
            SelectWinner();
        }
    }

    public void SelectWinner() {
        //Draw
        int winner = 0;
        if (player1Selection == player2Selection) {
            winner = 0;
        }
        //Player 1 wins
        else if (player1Selection == 0 && player2Selection == 1 
              || player1Selection == 1 && player2Selection == 2
              || player1Selection == 2 && player2Selection == 0) {
            winner = 1;
        }
        //Player 2 Wins
        else if (player2Selection == 0 && player1Selection == 1 
              || player2Selection == 1 && player1Selection == 2
              || player2Selection == 2 && player1Selection == 0) {
            winner = 2;
        }
        if (PV.IsMine) {
            PV.RPC("RPC_DeclareWinner", RpcTarget.AllBuffered, winner);
        }
    }

    [PunRPC] public void RPC_DeclareWinner(int winningPlayer) {
        //Declare winner
        winnerText.gameObject.SetActive(true);
        if (winningPlayer != 0) {
            winnerText.text = string.Format("Player {0} Won", winningPlayer);
        } else {
            winnerText.text = "Draw";
        }
        float waitTime = 3;
        while (waitTime > 0) {
            waitTime -= Time.deltaTime;
            Debug.Log(waitTime);
        }
        //winnerText.gameObject.SetActive(false);
        player1IsLocked = false;
        player2IsLocked = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
