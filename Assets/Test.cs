using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class Test : NetworkBehaviour
{
    public Text alienplayer;
    public Text haustierplayer;
    public GameObject alienCardsPanel;
    public GameObject petCardsPanel;

    public void Update()
    {
        if(CurrentGame.startGame)
        {
            gameStart();
            CurrentGame.startGame = false;
        }
    }
    public void gameStart()
    {

        loadTroopsPanel();
        Debug.Log("Test.cs: Start method");
    }

    private void loadTroopsPanel()
    {
        Debug.Log("loadTroops");
        if (CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.Haustiere.ToString())
        {
            Debug.Log("Random haustier");
            petCardsPanel.SetActive(true);
            alienCardsPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Random alien");
            petCardsPanel.SetActive(false);
            alienCardsPanel.SetActive(true);
        }
    }
    //Send Message To Server -> ServerRpc
    //Send Message From Client to Server -> ClientRpc


}
