using TMPro;
using UnityEngine;

public class EndUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text winnerMessage; // Reference to the TextMeshPro UI element

    private void Start()
    {
        // Display the winner's name
        if (winnerMessage != null)
        {
            winnerMessage.text = $"{GameManager.WinnerName} wins!";
        }
    }
}
