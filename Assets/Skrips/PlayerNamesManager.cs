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

    private void Start()
    {
        // Force an initial update of player names
        UpdatePlayerNames();
    }

    private void Update()
    {
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
        string petsPlayerName = CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value;
        string aliensPlayerName = CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value;

        // Instantiate and set the pets player name UI
        petsPlayerNameUI = Instantiate(petsPlayerNameTMPPrefab, petsPlayerNameParent);
        petsPlayerNameUI.GetComponent<TMP_Text>().text = petsPlayerName;

        // Instantiate and set the aliens player name UI
        aliensPlayerNameUI = Instantiate(aliensPlayerNameTMPPrefab, aliensPlayerNameParent);
        aliensPlayerNameUI.GetComponent<TMP_Text>().text = aliensPlayerName;
    }
}
