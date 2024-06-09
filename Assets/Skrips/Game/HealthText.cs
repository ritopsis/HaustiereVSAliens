using UnityEngine;
using TMPro;

public class HealthText : MonoBehaviour
{
    public TMP_Text healthText; // Reference to the TMP_Text component

    public void SetHealth(int currentHealth, int maxHealth)
    {
        healthText.text = $"Health: {currentHealth}/{maxHealth}";
    }
}