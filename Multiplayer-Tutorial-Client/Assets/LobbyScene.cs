using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LobbyScene : MonoBehaviour
{
    public static LobbyScene Instance;

    public TMP_Text welcomeMessage;

    public TMP_InputField ipAddress;

    public TMP_InputField createEmail;
    public TMP_InputField createUsername;
    public TMP_InputField createPassword;

    public TMP_InputField loginUsername;
    public TMP_InputField loginPassword;

    public TMP_Text authenticationMessage;

    void Start() {
        Instance = this;
    }

    public void OnClickCreateAccount() {

        EnableInputs(false);
        string email = createEmail.text;
        string password = createPassword.text;
        string username = createUsername.text;

        Client.Instance.SendCreateAccount(username, password, email);
    }

    public void OnClickLoginRequest() {

        EnableInputs(false);

        string email = createEmail.text;
        string password = createPassword.text;

        Client.Instance.SendLoginRequest(email, password);
    }
    public void OnClickSetIPAddress() {
        Client.Instance.SERVER_IP = ipAddress.text;
        Debug.Log(ipAddress.text);
        Debug.Log(Client.Instance.SERVER_IP);
        Client.Instance.Init();
    }

    public void SetWelcomeMessage(string msg) {
        welcomeMessage.text = msg;
    }

    public void SetAuthMessage(string msg) {
        authenticationMessage.text = msg;
    }

    public void EnableInputs(bool enable) {
        GameObject.Find("Canvas").GetComponent<CanvasGroup>().interactable = enable;
    }


}
