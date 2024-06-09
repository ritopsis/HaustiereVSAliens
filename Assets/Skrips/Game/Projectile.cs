using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public Transform graphics;

    public int damage;

    public float flySpeed, rotateSpeed;

    private Transform target; // The target the projectile will move towards

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
            collider.GetComponent<Alien>().LoseHealth(damage);
            Destroy(gameObject);
        }

        if (collider.CompareTag("Outbound"))
        {
            Debug.Log("Mothership hit");
            // Logic for mothership losing health
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
            transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // If there is no target, continue flying to the right
            transform.Translate(Vector3.right * flySpeed * Time.deltaTime, Space.World);
        }
    }
}
