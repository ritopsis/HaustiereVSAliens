using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Projectile : NetworkBehaviour
{
    public Transform graphics;
    public int damage;
    public float flySpeed, rotateSpeed;
    private Transform target;

    public void Init(int dmg)
    {
        damage = dmg;
    }

    public void Initialize(Transform target)
    {
        this.target = target;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!IsServer) return;  // Ensure only the server handles collisions

        if (collider.CompareTag("Alien"))
        {
            Debug.Log("Enemy hit");
            Alien alien = collider.GetComponent<Alien>();
            if (alien != null)
            {
                alien.LoseHealthServerRpc(damage);
                DestroyProjectileClientRpc();
            }
        }

        if (collider.CompareTag("Outbound"))
        {
            Debug.Log("Mothership hit");
            Base mothership = collider.GetComponent<Base>();
            if (mothership != null)
            {
                mothership.TakeDamage(damage);
            }
            DestroyProjectileClientRpc();
        }
    }

    void Update()
    {
        if (IsServer)
        {
            Rotate();
            FlyForward();
        }
    }

    void Rotate()
    {
        graphics.Rotate(new Vector3(0, 0, -rotateSpeed * Time.deltaTime));
    }

    void FlyForward()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector3.right * flySpeed * Time.deltaTime, Space.World);
        }
    }

    [ClientRpc]
    void DestroyProjectileClientRpc()
    {
        Destroy(gameObject);
    }
}




/*
public class Projectile : MonoBehaviour
{
    public Transform graphics;
    public int damage;
    public float flySpeed, rotateSpeed;
    private Transform target;

    public void Init(int dmg)
    {
        damage = dmg;
    }

    public void Initialize(Transform target)
    {
        this.target = target;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Alien"))
        {
            Debug.Log("Enemy hit");
            Alien alien = collider.GetComponent<Alien>();
            if (alien != null)
            {
                alien.LoseHealth(damage);
                Destroy(gameObject);
            }
        }

        if (collider.CompareTag("Outbound"))
        {
            Debug.Log("Mothership hit");
            Base mothership = collider.GetComponent<Base>();
            if (mothership != null)
            {
                mothership.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Rotate();
        FlyForward();
    }

    void Rotate()
    {
        graphics.Rotate(new Vector3(0, 0, -rotateSpeed * Time.deltaTime));
    }

    void FlyForward()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0;
            transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(Vector3.right * flySpeed * Time.deltaTime, Space.World);
        }
    }
}
*/