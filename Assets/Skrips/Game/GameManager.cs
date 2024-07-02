using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

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
            // Assuming there are only two bases, one for each player
            if (destroyedBase.CompareTag("House"))
            {
                DeclareWinner("Player 2");
            }
            else if (destroyedBase.CompareTag("UFO"))
            {
                DeclareWinner("Player 1");
            }
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
        Debug.Log(winner + " wins!");
        UIManager.Instance.DisplayWinner(winner);
        PauseGame();
    }

    private void DeclareWinner(string winner)
    {
        DeclareWinnerServerRpc(winner);
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
    }
}
