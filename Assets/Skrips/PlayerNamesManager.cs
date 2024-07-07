using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerNamesManager : MonoBehaviour
{
    public GameObject petsPlayerNameTMPPrefab; // Prefab for displaying the pets player name using TextMeshPro
    public GameObject aliensPlayerNameTMPPrefab; // Prefab for displaying the aliens player name using TextMeshPro
    public Transform petsPlayerNameParent; // Parent transform to hold pets player name UI element
    public Transform aliensPlayerNameParent; // Parent transform to hold aliens player name UI element

    private GameObject petsPlayerNameUI;
    private GameObject aliensPlayerNameUI;

    private string oldName = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
    private string oldNameOther = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;

    private void Start()
    {
        // Force an initial update of player names
        UpdatePlayerNames();
    }

    private void Update()
    {
        if(oldName != CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value || oldNameOther != CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value)
        {
            UpdatePlayerNames();
        }
        if(CurrentGame.updateName)
        {
            UpdatePlayerNames();
            CurrentGame.updateName = false;
        }
       /* if (LobbyManager.instance.updateUI)
        {
            UpdatePlayerNames();
            LobbyManager.instance.updateUI = false;
        }*/
    }

    public void UpdatePlayerNames()
    {
        // Clear existing player names UI if they exist
        if (petsPlayerNameUI != null)
        {
            Destroy(petsPlayerNameUI);
        }
        if (aliensPlayerNameUI != null)
        {
            Destroy(aliensPlayerNameUI);
        }

        // Fetch current player names from CurrentGame
        string currentPlayerName = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
        string otherPlayerName = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;


        if (CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.Haustiere.ToString())
        {
            petsPlayerNameUI = Instantiate(petsPlayerNameTMPPrefab, petsPlayerNameParent);
            petsPlayerNameUI.GetComponent<TMP_Text>().text = currentPlayerName;
            aliensPlayerNameUI = Instantiate(aliensPlayerNameTMPPrefab, aliensPlayerNameParent);
            aliensPlayerNameUI.GetComponent<TMP_Text>().text = otherPlayerName;
        }
        else
        {
            petsPlayerNameUI = Instantiate(petsPlayerNameTMPPrefab, petsPlayerNameParent);
            petsPlayerNameUI.GetComponent<TMP_Text>().text = otherPlayerName;
            aliensPlayerNameUI = Instantiate(aliensPlayerNameTMPPrefab, aliensPlayerNameParent);
            aliensPlayerNameUI.GetComponent<TMP_Text>().text = currentPlayerName;

        }
        Debug.Log("UPDATING NAME");
        oldName = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
        oldNameOther = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;
    }
}
