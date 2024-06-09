using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pet : NetworkBehaviour
{
    public int health;
    public int cost;
    private Transform spawnPoint;

    protected virtual void Start()
    {
        Debug.Log("Pet");
    }

    public virtual void Init(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    [ServerRpc]
    public void TakeDamageServerRpc(int amount)
    {
        TakeDamage(amount);
    }

    public void TakeDamage(int amount)
    {
        if (IsServer)
        {
            health -= amount;
            if (health <= 0)
            {
                Die();
            }
        }
    }

    protected virtual void Die()
    {
        Debug.Log("Pet died");
        if (IsServer)
        {
            FindObjectOfType<SpawnerPet>().RevertSpawnPoint(spawnPoint);
            Destroy(gameObject);
        }
    }
}




/*

public class Pet : MonoBehaviour
{
    public int health;
    public int cost;
    private Transform spawnPoint; // Changed from Vector3Int to Transform

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Debug.Log("Pet");
    }

    public virtual void Init(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint; // Store the spawn point transform
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
        Debug.Log("Pet died");
        FindObjectOfType<SpawnerPet>().RevertSpawnPoint(spawnPoint); // Revert the spawn point state
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
}
*/