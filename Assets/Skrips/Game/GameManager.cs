using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public static string WinnerName = null; // Static variable to hold the winner's name


    private bool gameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void BaseDestroyed(Base destroyedBase)
    {
        if (IsServer && !gameOver)
        {

            gameOver = true;
            if(CurrentGame.win)
            {
                DeclareWinner(CurrentGame.currentPlayer.Data[LobbyManager.KEY_USERNAME].Value);
            }
            else
            {
                DeclareWinner(CurrentGame.otherPlayer.Data[LobbyManager.KEY_USERNAME].Value);
            }
            // Assuming there are only two bases, one for each player
            /*if (destroyedBase.CompareTag("House"))
            {
                DeclareWinner("Player 2");
            }
            else if (destroyedBase.CompareTag("UFO"))
            {
                DeclareWinner("Player 1");
            }*/
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeclareWinnerServerRpc(string winner)
    {
        gameOver = true;
        DeclareWinnerClientRpc(winner);
    }

    [ClientRpc]
    private void DeclareWinnerClientRpc(string winner)
    {
        // Display winner to all clients

        WinnerName = winner;
        Debug.Log(winner + " wins!");
        UIManager.Instance.DisplayWinner(winner);
        //PauseGame();
        LoadEndScene();
    }

    private void DeclareWinner(string winner)
    {
        DeclareWinnerServerRpc(winner);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }

      private void LoadEndScene()
    {

        Debug.Log("Loading EndScene...");
        Time.timeScale = 1;
        // Shutdown the network manager
        NetworkManager.Singleton.Shutdown();
        // Load the end scene
        SceneManager.LoadScene("EndScene"); // Replace with your end scene name
    }
}
