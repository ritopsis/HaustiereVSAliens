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
    public GameObject places;
    public CharacterObjectList col;


    public GameObject tri;
    public static int count;

    void Start()
    {
        count = 0;
        if(CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.Haustiere.ToString())
        {
            haustierplayer.text = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
            alienplayer.text = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;
        }
        else
        {
            alienplayer.text = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
            haustierplayer.text = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;
        }
    }


    public void starthost()
    {
        Debug.Log("start");
        NetworkManager.Singleton.StartHost();
    }
    public void startclient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void SendMessageToServer(string position)
    {

        Debug.Log("sending");
        ServerRpc(col.characterObjectList.IndexOf(tri), position);
            Debug.Log("send");

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
