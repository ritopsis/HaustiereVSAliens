using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet_Doge : Pet
{
    public GameObject projectilePrefab; // Prefab of the projectile to shoot
    public float attackCooldown = 1f; // Time between attacks
    public float attackRange = 5f; // Range within which the doge can attack
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
        Vector2 rightBoundary = new Vector2(position.x + attackRange, position.y + 1);
        Collider2D[] hitTargets = Physics2D.OverlapAreaAll(position, rightBoundary);

        foreach (Collider2D target in hitTargets)
        {
            if (target.CompareTag("Alien") && target.transform.position.x > transform.position.x) // Ensure the target is to the right
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
        Vector3 rightBoundary = new Vector3(position.x + attackRange, position.y + 1, position.z);
        Gizmos.DrawLine(position, rightBoundary);
    }
}
