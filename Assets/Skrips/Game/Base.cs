
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Base : NetworkBehaviour
{
    public int maxHealth;
    private NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    public TMP_Text healthText; // Reference to the HealthText script

    void Start()
    {
        
        Debug.Log("base start");
        currentHealth.Value = maxHealth;
        UpdateHealthText();
        currentHealth.OnValueChanged += OnHealthChanged;
    }

    public void TakeDamage(int damage)
    {
        if (IsServer)
        {
            Debug.Log("base take damage");
            currentHealth.Value -= damage;
            if (currentHealth.Value <= 0)
            {
                if(gameObject.name == "House" && CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.Haustiere.ToString())
                {
                    CurrentGame.win = false;
                }
                else if(gameObject.name == "UFO" && CurrentGame.currentPlayer.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.Aliens.ToString())
                {
                    CurrentGame.win = false;
                }
                else
                {
                    CurrentGame.win = true;
                }
                DestroyBase();
            }
        }
    }

    void DestroyBase()
    {
        Debug.Log(gameObject.name + " has been destroyed!");

        NotifyGameManager();
        // Add additional logic for when the base is destroyed
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        UpdateHealthText();
    }


    void UpdateHealthText()
    {
    if (healthText != null)
    {
        healthText.text = $"{currentHealth.Value}/{maxHealth}";
    }
    }


    void NotifyGameManager()
    {
        GameManager.Instance.BaseDestroyed(this);
    }
}




/*public class Base : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public string targetTag; // Tag for identifying projectiles or aliens
    public HealthText healthText; // Reference to the HealthText script

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthText();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " health: " + currentHealth);
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            DestroyBase();
        }
    }

    void DestroyBase()
    {
        Debug.Log(gameObject.name + " has been destroyed!");
        // Add additional logic for when the base is destroyed
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag(targetTag))
        {
            if (targetTag == "Alien")
            {
                Alien alien = collider.GetComponent<Alien>();
                if (alien != null)
                {
                    TakeDamage(alien.attackPower);
                    Destroy(collider.gameObject); // Destroy the alien when it hits the base
                }
            }
            else if (targetTag == "Projectile")
            {
                Projectile projectile = collider.GetComponent<Projectile>();
                if (projectile != null)
                {
                    TakeDamage(projectile.damage);
                    Destroy(collider.gameObject); // Destroy the projectile when it hits the base
                }
            }
        }
    }

    void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.SetHealth(currentHealth, maxHealth);
        }
    }
}
*/