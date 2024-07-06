using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class Test : NetworkBehaviour
{
    // Start is called before the first frame update
    public Text alienplayer;
    public Text haustierplayer;
    public GameObject alienCardsPanel;
    public GameObject petCardsPanel;
    public GameObject places;
    public CharacterObjectList col;


    public GameObject tri;
    public static int count;

    void Start()
    {
        count = 0;
    }
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

    public void starthost()
    {
        Debug.Log("start");
        CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value = LobbyManager.PlayerCharacter.Haustiere.ToString();
        CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value = "Host";
        gameStart();
    }
    public void startclient()
    {
        CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value = LobbyManager.PlayerCharacter.Aliens.ToString();
        CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value = "Client";
        gameStart();
    }

    public void SendMessageToServer(string position)
    {

        Debug.Log("sending");
        ServerRpc(col.characterObjectList.IndexOf(tri), position);
        Debug.Log("send");

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


    //send from client to server
    [ServerRpc(RequireOwnership =false)]
    private void ServerRpc(int index, string position)
    {
        Debug.Log("Start: " + position);
        GameObject a = col.characterObjectList[index];
        foreach (Transform child in places.transform)
        {
            if (child.name == position)
            {
                Debug.Log("Child found");
                a.transform.position = new Vector3(child.transform.position.x, child.transform.position.y,0);
                Debug.Log("new position");
            }
        }
        GameObject newSpawn = Instantiate(a);
        newSpawn.GetComponent<NetworkObject>().Spawn(true);
        ClientRpc(index);                    
    }

    //from server to all client
    [ClientRpc]
    private void ClientRpc(int index)
    {
        //Debug.Log(OwnerClientId);
        //Instantiate(col.characterObjectList[index]);
    }

}
