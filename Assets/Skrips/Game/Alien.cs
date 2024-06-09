using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public int health;
    public int cost;
    private Transform spawnPoint;

    public float speed = 2f; // Speed of the alien
    public int attackPower = 10; // Attack power of the alien
    public float attackRange = 0.5f; // Range within which the alien can attack

    private bool isAttacking = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Debug.Log("Alien");
        // Start moving the alien
        StartCoroutine(Move());
    }

    public virtual void Init(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    IEnumerator Move()
    {
        while (!isAttacking)
        {
            // Move the alien from right to left
            transform.position += Vector3.left * speed * Time.deltaTime;

            // Check if there is any pet within attack range
            CheckAttack();

            yield return null;
        }
    }

    void CheckAttack()
    {
        // Detect any potential pets within the attack range
        Collider2D[] hitPets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D pet in hitPets)
        {
            if (pet.CompareTag("Pet")) // Ensure the pet has the correct tag
            {
                isAttacking = true;
                StartCoroutine(Attack(pet.GetComponent<Pet>()));
                break;
            }
        }
    }

    IEnumerator Attack(Pet pet)
    {
        while (pet != null && health > 0)
        {
            pet.TakeDamage(attackPower);
            yield return new WaitForSeconds(1f); // Attack every second
        }
        isAttacking = false;
    }

    public virtual bool LoseHealth(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }

    protected virtual void Die()
    {
        Debug.Log("Alien died");
        FindObjectOfType<SpawnerAlien>().RevertSpawnPoint(spawnPoint); // Revert the spawn point state
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Visualize the attack range in the editor
    }
}
