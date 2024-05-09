using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public Text username;
    public GameObject LobbyRooms;
    public GameObject LobbyCreation;
    public GameObject MyLobby;
    public GameObject LobbyFull;

    //LobbyRooms
    public GameObject LobbyContent;
    public GameObject Lobbyitem;

    public GameObject Player1Haustiere;
    public GameObject Player1Aliens;
    public GameObject Player2Haustiere;
    public GameObject Player2Aliens;


    private void Awake()
    {
        username.text = LobbyManager.instance.username;
    }
    private void Update()
    {
        if (LobbyManager.instance.pullenlobby) {
            LobbyManager.instance.pullenlobby = false;
            UpdateLobbyUIItems();
        }
        if(LobbyManager.instance.updateUI)
        {
            LobbyManager.instance.updateUI = false;
            UpdatePlayerUI();
        }
    }
    public void UpdateLobbyUIItems()
    {
        foreach (Transform child in LobbyContent.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Lobby lobby in LobbyManager.instance.lobbies.Results)
        {
            GameObject item = Lobbyitem;
            GameObject newObject = Instantiate(item, LobbyContent.transform);
            Text childText = newObject.transform.Find("Name").GetComponent<Text>();
            if (childText != null)
            {
                childText.text = lobby.Name + lobby.Id;
            }
            Button joinButton = newObject.transform.Find("Join").GetComponent<Button>();
            if (joinButton != null)
            {
                joinButton.onClick.AddListener(() =>
                {
                    LobbyManager.instance.JoinLobbyByIdAsync(lobby);
                    if(LobbyManager.instance.activeLobby != null)
                    {
                        LobbyRooms.SetActive(false);
                        MyLobby.SetActive(true);
                    }
                    else
                    {
                        UpdateLobbyRoom();
                        LobbyFull.SetActive(true);
                    }
                });
            }
        }

    }
    public void OpenLobbyCreation()
    {
        LobbyCreation.gameObject.SetActive(true);
    }
    public void CloseLobbyCreation()
    {
        LobbyCreation.gameObject.SetActive(false);
    }
    public void CreateLobby(Text lobbyname)
    {
        LobbyManager.instance.CreateLobby(Functions.UserInputCheck(lobbyname.text, 3, 6));
        LobbyCreation.SetActive(false);
        LobbyRooms.SetActive(false);
        MyLobby.SetActive(true);

    }
    public void CloseMyLobby()
    {
        LobbyRooms.SetActive(true);
        MyLobby.SetActive(false);
        LobbyManager.instance.LeaveLobby();   
    }
    public void CloseLobbyFull()
    {
        LobbyFull.SetActive(false);

    }
    public void UpdateLobbyRoom()
    {
        LobbyManager.instance.ListLobbies();
    }
    public void UpdatePlayerCharacter(string Text)
    {
        LobbyManager.PlayerCharacter character = LobbyManager.PlayerCharacter.Haustiere;
        if (Text == "Aliens")
        {
            character = LobbyManager.PlayerCharacter.Aliens;
        }
        foreach (var players in LobbyManager.instance.activeLobby.Players)
        {
            if(players.Id == LobbyManager.instance.playerid)
            {
                if(players.Data[LobbyManager.KEY_USERNAME].Value.ToString() != character.ToString())
                {
                    LobbyManager.instance.UpdatePlayerCharacter(character);
                    break;
                }
            }
        }
        UpdatePlayerUI();

    }
    public void UpdatePlayerUI()
    {
        Player1Haustiere.SetActive(false);
        Player1Aliens.SetActive(false);
        Player2Haustiere.SetActive(false);
        Player2Aliens.SetActive(false);
        int count = 1;
        foreach (var players in LobbyManager.instance.activeLobby.Players)
        {
            if(count == 1)
            {
               if (players.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value.ToString() == LobbyManager.PlayerCharacter.Haustiere.ToString())
                {
                    Text childText = Player1Haustiere.transform.Find("Name").GetComponent<Text>();
                    if (childText != null)
                    {
                        childText.text = players.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                    }
                    Player1Aliens.SetActive(false);
                    Player1Haustiere.SetActive(true);
                }
               else
                {
                    Text childText = Player1Aliens.transform.Find("Name").GetComponent<Text>();
                    if (childText != null)
                    {
                        childText.text = players.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                    }
                    Player1Haustiere.SetActive(false);
                    Player1Aliens.SetActive(true);
                }
               count++;
                
            }
            else
            {
                if (players.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value.ToString() == LobbyManager.PlayerCharacter.Haustiere.ToString())
                {
                    Text childText = Player2Haustiere.transform.Find("Name").GetComponent<Text>();
                    if (childText != null)
                    {
                        childText.text = players.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                    }
                    Player2Aliens.SetActive(false);
                    Player2Haustiere.SetActive(true);
                }
                else
                {
                    Text childText = Player2Aliens.transform.Find("Name").GetComponent<Text>();
                    if (childText != null)
                    {
                        childText.text = players.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                    }
                    Player2Haustiere.SetActive(false);
                    Player2Aliens.SetActive(true);
                }
            }
            
        }
    }

}
