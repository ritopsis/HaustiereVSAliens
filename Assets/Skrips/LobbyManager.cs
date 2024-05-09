using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance { get; private set; }
    private void Awake()
    {
        instance = this;
    }
    public const string KEY_USERNAME = "Username";
    public const string KEY_PLAYER_CHARACTER = "Character";

    public enum PlayerCharacter
    {
        Haustiere,
        Aliens
    }

    private Player GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_USERNAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, username) },
            { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Haustiere.ToString()) }
        });
    }


    public string username { get; private set; }
    public string playerid { get; private set; }

    public QueryResponse lobbies { get; private set; } //response of the current lobby request -> to display in UI
    public Lobby activeLobby { get; private set; }
    public bool lobbyjoining = false; //after trying to join a lobby = true
    public bool updateUI = false; //after getting new lobbydata = true
    public bool pullenlobby = false; //after requesting current lobbies = true

    private float heartbeatTimer; //needed because after 30sec -> "inactive" lobby gets destroyed!
    private float lobbyPollTimer; //timer to request lobbydata
    private float refreshLobbyListTimer;


    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPolling();
        //HandleRefreshLobbyList();
    }

    private async void HandleLobbyHeartbeat()
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= Time.deltaTime; //seconds since the last frame
            if (heartbeatTimer < 0f)
            {
                heartbeatTimer = 15f; //reset to 15sec because after 30 it would destroy lobby!
                Debug.Log("Heartbeat");
                await LobbyService.Instance.SendHeartbeatPingAsync(activeLobby.Id);
            }
        }
    }

    private async void HandleLobbyPolling()
    {
        if (activeLobby != null)
        {
            lobbyPollTimer -= Time.deltaTime;
            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 2.0f; //time to get newest lobby information
                lobbyPollTimer = lobbyPollTimerMax;

                activeLobby = await LobbyService.Instance.GetLobbyAsync(activeLobby.Id);
                updateUI = true;
            }
        }
    }
    private void HandleRefreshLobbyList()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
        {
            refreshLobbyListTimer -= Time.deltaTime;
            if (refreshLobbyListTimer < 0f)
            {
                float refreshLobbyListTimerMax = 5f;
                refreshLobbyListTimer = refreshLobbyListTimerMax;

                ListLobbies();
            }
        }
    }
    public bool IsLobbyHost() //to check if the current user is the host of the lobby and if there is a lobby!
    {
        return activeLobby != null && activeLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public async void Authenticate(string username)
    {
        this.username = username;
        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(username);

        await UnityServices.InitializeAsync(initializationOptions); //Initialize API (send Request)

        AuthenticationService.Instance.SignedIn += () => { 
            // Signed in
            Debug.Log("Signed in! " + AuthenticationService.Instance.PlayerId);
            playerid = AuthenticationService.Instance.PlayerId;
            ListLobbies();
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync(); //Sign in Anonymously -> no account needed
    }
    public async void CreateLobby(string lobbyName)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = GetPlayer() //giving the username to lobby
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2,options); //Name, maxPlayer
            activeLobby = lobby;
            updateUI = true;
            Debug.Log("Created Lobby " + lobby.Name);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            lobbyjoining = true;
        }
    }
    public async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 10; //how much are shown

            //filter for available lobbies only
            options.Filters = new List<QueryFilter> {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            //order by newest lobbies first
            options.Order = new List<QueryOrder> {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);//Name, maxPlayer
            pullenlobby = true;
            lobbies = queryResponse;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }
    public async void JoinLobbyByIdAsync(Lobby lobby)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = GetPlayer() //giving the username from joining player to lobby
            };
            activeLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            updateUI = true;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally { 
        lobbyjoining = true; 
        }
       

    }
    public async void UpdatePlayerCharacter(PlayerCharacter playerCharacter)
    {
        if (activeLobby != null)
        {
            try
            {
                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>() {
                    {
                        KEY_PLAYER_CHARACTER, new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Public,
                            value: playerCharacter.ToString())
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(activeLobby.Id, playerId, options);
                activeLobby = lobby;
                updateUI = true;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void LeaveLobby()
    {
        if (activeLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(activeLobby.Id, AuthenticationService.Instance.PlayerId);
                activeLobby = null;
                ListLobbies();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(activeLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    public bool IsPlayerInLobby() //check if the player is in any lobby
    {
        if (activeLobby != null && activeLobby.Players != null)
        {
            foreach (Player player in activeLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    return true;  // This player is in this lobby
                }
            }
        }
        return false;
    }
}
