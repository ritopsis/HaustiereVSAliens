using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
   public Transform graphics;

   public int damage;

   public float flySpeed, rotateSpeed;

   public void Init(int dmg)
   {
    damage = dmg;
   }

   private void OnTriggerEnter2D(Collider2D collider)
   {

    if (collider.tag == "Alien")
    {
        Debug.Log("Enemy hit");
        collider.GetComponent<Alien>().LoseHealth();
        Destroy(gameObject);
    }

    if (collider.tag =="Outbound")
    {
        Debug.Log("Mothership hit");
        //mamaship losehealth 
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
    graphics.Rotate(new Vector3(0,0, -rotateSpeed*Time.deltaTime));

   }

   void FlyForward()
   {
    transform.Translate(transform.right * flySpeed * Time.deltaTime);
   }
   

}
