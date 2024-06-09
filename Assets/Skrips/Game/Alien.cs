using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Alien : MonoBehaviour
{

    public int health, attackPower;
    public float moveSpeed;
    private Rigidbody2D rb;
    
    //public Animator animator;
    public float attackInterval;
    Coroutine attackOrder;
    Pet detectedPet;

    // Start is called before the first frame update

    private Vector3Int cellPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }


    public virtual void Init(Vector3Int cellPos)
    {
        cellPosition =cellPos;

    }

    // Update is called once per frame
    void Update()
    {
        if(!detectedPet)
        {
            Move();
        }
        
    }

    IEnumerator Attack()
    {
        //animator.Play("Attack", 0,0);
        yield return new WaitForSeconds(attackInterval);
        
        attackOrder= StartCoroutine(Attack());
    }

    void Move()
    {
        //animator.Play("Move");
        //transform.Translate(-transform.right*moveSpeed*Time.deltaTime);
        rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    }

    public void InflictDamage()
    {
        bool petDied = detectedPet.LoseHealth(attackPower);
        if (petDied)
        {
            detectedPet = null;
            StopCoroutine(attackOrder);
        }

    }

    public void LoseHealth()
    {
        health--;

        StartCoroutine(BlinkRed());

        if (health<=0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator BlinkRed()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);

        GetComponent<SpriteRenderer>().color= Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {

    
        /*if (detectedPet)
            return;

        if (collider.tag == "Pet")
        { 
            moveSpeed = 0;
            detectedPet = collider.GetComponent<Pet>();
            attackOrder = StartCoroutine(Attack());
        }*/

         if (collider.CompareTag("Pet") )//&& !detectedPet)
        {
            moveSpeed = 0;
            detectedPet = collider.GetComponent<Pet>();
            attackOrder = StartCoroutine(Attack());
        }
        else if (collider.CompareTag("Alien"))
        {
            // Handle collision with another alien
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), false);
        }
    }


    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Pet") && detectedPet == collider.GetComponent<Pet>())
        {
            moveSpeed = 1; // Resume moving when pet is out of range
            detectedPet = null;
            if (attackOrder != null)
            {
                StopCoroutine(attackOrder);
            }
        }
    }

    /* void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pet"))
        {
            // Stop moving and start attacking the plant
            
            rb.velocity = Vector2.zero;
            // You can add more logic here to reduce the plant's health
        }
    }

     void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pet"))
        {
            // Resume movement after the plant is destroyed
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
    }*/

}
