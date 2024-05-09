using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Text alienplayer;
    public Text haustierplayer;
    public GameObject aliens;

    void Start()
    {
        if(CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.Haustiere.ToString())
        {
            haustierplayer.text = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
            alienplayer.text = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;
        }
        else
        {
            alienplayer.text = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
            haustierplayer.text = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;
            aliens.SetActive(true);
        }
    }

}
