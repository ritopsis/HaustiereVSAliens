using UnityEngine;

public class Base : MonoBehaviour
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
