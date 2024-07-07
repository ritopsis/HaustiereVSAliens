using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using static LobbyManager;

public class CurrentGame : MonoBehaviour
{
    public static Player currentPlayer = new Player(null, null, new Dictionary<string, PlayerDataObject> {
            { KEY_USERNAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Player 1") },
            { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Haustiere.ToString())}});
    public static Player otherPlayer = new Player(null, null, new Dictionary<string, PlayerDataObject> {
            { KEY_USERNAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Player 2") },
            { KEY_PLAYER_CHARACTER, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerCharacter.Aliens.ToString()) }});

    public static bool startGame;

    public static bool win;

    public static bool updateName;
}
