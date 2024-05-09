using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public Text username;
    private bool clickable = true;

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
    public GameObject Player2HaustiereKick;
    public GameObject Player2AliensKick;


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
            StillInLobby();
            UpdatePlayerUI();
        }
        if (LobbyManager.instance.lobbyjoining) {
            LobbyManager.instance.lobbyjoining = false;
            LobbyJoining();
        }
    }
    public void StillInLobby() //check is player is in lobby -> could be kicked from host
    {
        if(!LobbyManager.instance.IsPlayerInLobby()) //is not in lobby anymore
        {
            Show(LobbyRooms);
            Hide(MyLobby);
        }
    }
    public void LobbyJoining() //after player tried joining a lobby
    {
        if (LobbyManager.instance.activeLobby != null) //check if the lobby joining was successful
        {
            Hide(LobbyRooms);
            Show(MyLobby);
        }
        else //joining was not successful
        {
            UpdateLobbyRoom(); //update to show the available lobbyrooms
            Show(LobbyFull); //UI show that lobby is full
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
                childText.text = lobby.Name;
            }
            Button joinButton = newObject.transform.Find("Join").GetComponent<Button>();
            if (joinButton != null)
            {
                joinButton.onClick.AddListener(() =>
                {
                    LobbyManager.instance.JoinLobbyByIdAsync(lobby);
                });
            }
        }

    }
    public void OpenLobbyCreation()
    {
        Show(LobbyCreation);
    }
    public void CloseLobbyCreation()
    {
        Hide(LobbyCreation);
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
        Show(LobbyRooms);
        Hide(MyLobby);
        LobbyManager.instance.LeaveLobby();   
    }
    public void CloseLobbyFull() //to close the popup "Lobby is already full"
    {
        Hide(LobbyFull);

    }
    public void UpdateLobbyRoom() //request current lobbys  
    {
        LobbyManager.instance.ListLobbies();
    }
    public void UpdatePlayerCharacter(string Text) //to switch between factions
    {
        if(clickable)
        {
            LobbyManager.PlayerCharacter character = LobbyManager.PlayerCharacter.Haustiere;
            if (Text == "Aliens")
            {
                character = LobbyManager.PlayerCharacter.Aliens;
            }
            foreach (var players in LobbyManager.instance.activeLobby.Players)
            {
                if (players.Id == LobbyManager.instance.playerId)
                {
                    if (players.Data[LobbyManager.KEY_USERNAME].Value.ToString() != character.ToString())
                    {
                        LobbyManager.instance.UpdatePlayerCharacter(character);
                        break;
                    }
                }
            }
            UpdatePlayerUI();
        }
    }
    public void KickPlayer() //kick player as host of the lobby
    {
        LobbyManager.instance.KickPlayer(LobbyManager.instance.activeLobby.Players[1].Id); //Players[O] is LobbyOwner, 1 is Second Player
    }
    public void UpdatePlayerUI()
    {
        if(LobbyManager.instance.activeLobby != null)
        {
            Hide(Player1Haustiere);
            Hide(Player1Aliens);
            Hide(Player2Haustiere);
            Hide(Player2Aliens);
            Hide(Player2HaustiereKick);
            Hide(Player2AliensKick);
            int count = 1;
            Text lobbyname = MyLobby.transform.Find("Name").GetComponent<Text>();
            if (lobbyname != null)
            {
                lobbyname.text = LobbyManager.instance.activeLobby.Name;
            }
            foreach (Player player in LobbyManager.instance.activeLobby.Players)
            {
                if (count == 1)
                {
                    if (player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value.ToString() == LobbyManager.PlayerCharacter.Haustiere.ToString())
                    {
                        Text childText = Player1Haustiere.transform.Find("Name").GetComponent<Text>();
                        if (childText != null)
                        {
                            childText.text = player.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                        }
                        Show(Player1Haustiere);
                    }
                    else
                    {
                        Text childText = Player1Aliens.transform.Find("Name").GetComponent<Text>();
                        if (childText != null)
                        {
                            childText.text = player.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                        }
                        Show(Player1Aliens);
                    }
                    count++;
                }
                else
                {
                    if (player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value.ToString() == LobbyManager.PlayerCharacter.Haustiere.ToString())
                    {
                        Text childText = Player2Haustiere.transform.Find("Name").GetComponent<Text>();
                        if (childText != null)
                        {
                            childText.text = player.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                        }
                        Player2Haustiere.SetActive(true);
                        if (LobbyManager.instance.IsLobbyHost()) //only host can kick
                        {
                            Show(Player2HaustiereKick);
                        }
                    }
                    else
                    {
                        Text childText = Player2Aliens.transform.Find("Name").GetComponent<Text>();
                        if (childText != null)
                        {
                            childText.text = player.Data[LobbyManager.KEY_USERNAME].Value.ToString();
                        }
                        Player2Aliens.SetActive(true);
                        if (LobbyManager.instance.IsLobbyHost()) //only host can kick
                        {
                            Show(Player2AliensKick);
                        }
                    }
                }
            }
        }
       
    }
    public void StartGame()
    {
        if (LobbyManager.instance.activeLobby.Players.Count == 2) {
            clickable = false;
            if (LobbyManager.instance.activeLobby.Players[0].Data[LobbyManager.KEY_PLAYER_CHARACTER].Value !=
                LobbyManager.instance.activeLobby.Players[1].Data[LobbyManager.KEY_PLAYER_CHARACTER].Value)
            {
                LobbyManager.instance.StartGame();
            }
        }
    }
    public void Hide(GameObject gameobject)
    {
        gameobject.SetActive(false);
    }
    public void Show(GameObject gameobject)
    {
        gameobject.SetActive(true);
    }

}
