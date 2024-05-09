using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserCreationUI : MonoBehaviour
{
    public Text userInput;
    public GameObject lobbyUI;

    public void CreateUser()
    {
        LobbyManager.instance.Authenticate(Functions.UserInputCheck(userInput.text,3,10));
        this.gameObject.SetActive(false);
        lobbyUI.gameObject.SetActive(true);
    }
}
