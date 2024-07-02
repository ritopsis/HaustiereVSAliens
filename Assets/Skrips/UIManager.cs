using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text winnerText; // Reference to the TextMeshPro UI element

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

    private void Start()
    {
        // Hide the winner text at the start of the game
        winnerText.gameObject.SetActive(false);
    }

    public void DisplayWinner(string winner)
    {
        // Show the winner text and set its content
        winnerText.text = $"{winner} wins!";
        winnerText.gameObject.SetActive(true);
    }
}
