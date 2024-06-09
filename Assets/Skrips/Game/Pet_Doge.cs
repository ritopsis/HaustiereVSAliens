using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet_Doge : Pet
{

    public int damage;

    public GameObject prefab_projectile;

    public float interval;

    // Start is called before the first frame update
    protected override void Start()
    {
        Debug.Log("Doge");

        StartCoroutine(ShootDelay());
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(interval);
        ShootItem();
        StartCoroutine(ShootDelay());
    }

    void ShootItem()
    {
        GameObject thrownProjectile = Instantiate(prefab_projectile,transform);

        thrownProjectile.GetComponent<Projectile>().Init(damage);
    }

    
}
