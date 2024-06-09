using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public int health;
    public int cost;
    public int attackPower = 10; // Attack power of the alien
    private Transform spawnPoint;

    public float speed = 2f;
    public float attackRange = 0.5f;

    private bool isAttacking = false;
    private Coroutine moveCoroutine;

    protected virtual void Start()
    {
        Debug.Log("Alien");
        StartMoving();
    }

    public virtual void Init(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    private void StartMoving()
    {
        moveCoroutine = StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            if (!isAttacking)
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
                CheckAttack();
            }
            yield return null;
        }
    }

    void CheckAttack()
    {
        Collider2D[] hitPets = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D pet in hitPets)
        {
            if (pet.CompareTag("Pet"))
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
            yield return new WaitForSeconds(1f);
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
        FindObjectOfType<SpawnerAlien>().RevertSpawnPoint(spawnPoint);
        if (gameObject.name == "DittoAlien")
        {
            FindObjectOfType<SpawnerAlien>().NotifyDittoDeath(spawnPoint);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Outbound"))
        {
            Debug.Log("Alien reached outbound");
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
