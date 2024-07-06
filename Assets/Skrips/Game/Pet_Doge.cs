using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pet_Doge : Pet
{
    public GameObject projectilePrefab; // Prefab of the projectile to shoot
    public float attackCooldown; // Time between attacks
    public float attackRange; // Range within which the doge can attack
    public int attackPower; // Attack power of the doge
    private float lastAttackTime;
    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lastAttackTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer && Time.time - lastAttackTime >= attackCooldown)
        {
            CheckAndAttack();
        }
    }

    void CheckAndAttack()
    {
        Vector2 position = transform.position;
        Vector2 topRight = new Vector2(position.x + attackRange, position.y + 0.5f);
        Vector2 bottomLeft = new Vector2(position.x, position.y - 0.5f);

        Collider2D[] hitTargets = Physics2D.OverlapAreaAll(bottomLeft, topRight);

        foreach (Collider2D target in hitTargets)
        {
            if ((target.CompareTag("Alien") || target.CompareTag("UFO")) && target.transform.position.x > transform.position.x)
            {
                FireProjectileServerRpc(target.GetComponent<NetworkObject>().NetworkObjectId);
                lastAttackTime = Time.time;
                break;
            }
        }
    }
    [ServerRpc]
    void FireProjectileServerRpc(ulong targetNetworkObjectId)
    {
        // Instantiate the projectile on the server
        var target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetNetworkObjectId].transform;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<NetworkObject>().Spawn();
        projectile.GetComponent<Projectile>().Initialize(target);
        projectile.GetComponent<Projectile>().Init(attackPower);

        // Get the projectile's network object ID
        ulong projectileNetworkObjectId = projectile.GetComponent<NetworkObject>().NetworkObjectId;

        // Inform clients to initialize the projectile
        FireProjectileClientRpc(targetNetworkObjectId, projectileNetworkObjectId);
    }

    [ClientRpc]
    void FireProjectileClientRpc(ulong targetNetworkObjectId, ulong projectileNetworkObjectId)
    {
        // Get the target transform on the client
        var target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetNetworkObjectId].transform;

        // Get the instantiated projectile on the client using its network object ID
        var projectile = NetworkManager.Singleton.SpawnManager.SpawnedObjects[projectileNetworkObjectId].GetComponent<Projectile>();

        // Perform client-specific initialization
        projectile.Initialize(target);
        projectile.Init(attackPower);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = transform.position;
        Vector3 topRight = new Vector3(position.x + attackRange, position.y + 0.5f, position.z);
        Vector3 bottomLeft = new Vector3(position.x, position.y - 0.5f, position.z);

        Gizmos.DrawLine(bottomLeft, new Vector3(topRight.x, bottomLeft.y, position.z));
        Gizmos.DrawLine(bottomLeft, new Vector3(bottomLeft.x, topRight.y, position.z));
        Gizmos.DrawLine(topRight, new Vector3(topRight.x, bottomLeft.y, position.z));
        Gizmos.DrawLine(topRight, new Vector3(bottomLeft.x, topRight.y, position.z));
    }
}





/*
public class Pet_Doge : Pet
{
    public GameObject projectilePrefab; // Prefab of the projectile to shoot
    public float attackCooldown = 1f; // Time between attacks
    public float attackRange = 10f; // Range within which the doge can attack
    public int attackPower = 10; // Attack power of the doge
    private float lastAttackTime;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lastAttackTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            CheckAndAttack();
        }
    }

    void CheckAndAttack()
    {
        Vector2 position = transform.position;
        Vector2 topRight = new Vector2(position.x + attackRange, position.y + 0.5f);
        Vector2 bottomLeft = new Vector2(position.x, position.y - 0.5f);

        Collider2D[] hitTargets = Physics2D.OverlapAreaAll(bottomLeft, topRight);

        foreach (Collider2D target in hitTargets)
        {
            if ((target.CompareTag("Alien") || target.CompareTag("Outbound")) && target.transform.position.x > transform.position.x)
            {
                FireProjectile(target.transform);
                lastAttackTime = Time.time;
                break;
            }
        }
    }

    void FireProjectile(Transform target)
    {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Initialize(target);
        projectile.GetComponent<Projectile>().Init(attackPower);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 position = transform.position;
        Vector3 topRight = new Vector3(position.x + attackRange, position.y + 0.5f, position.z);
        Vector3 bottomLeft = new Vector3(position.x, position.y - 0.5f, position.z);

        Gizmos.DrawLine(bottomLeft, new Vector3(topRight.x, bottomLeft.y, position.z));
        Gizmos.DrawLine(bottomLeft, new Vector3(bottomLeft.x, topRight.y, position.z));
        Gizmos.DrawLine(topRight, new Vector3(topRight.x, bottomLeft.y, position.z));
        Gizmos.DrawLine(topRight, new Vector3(bottomLeft.x, topRight.y, position.z));
    }
}
*/