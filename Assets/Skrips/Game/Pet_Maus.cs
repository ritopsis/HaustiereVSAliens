using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Pet_Maus : Pet
{
    public int attackPower; // Attack power of the Maus
    public float attackRange; // Range within which the Maus can attack
    public float attackInterval; // Time between attacks

    private Coroutine attackCoroutine;

    protected override void Start()
    {
        base.Start();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer && other.CompareTag("Alien"))
        {
            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackAllAliensInRange());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsServer && other.CompareTag("Alien"))
        {
            Collider2D[] remainingAliens = Physics2D.OverlapCircleAll(transform.position, attackRange);
            bool hasRemainingAliens = false;
            foreach (Collider2D alien in remainingAliens)
            {
                if (alien.CompareTag("Alien"))
                {
                    hasRemainingAliens = true;
                    break;
                }
            }

            if (!hasRemainingAliens)
            {
                if (attackCoroutine != null)
                {
                    StopCoroutine(attackCoroutine);
                    attackCoroutine = null;
                }
            }
        }
    }

    private IEnumerator AttackAllAliensInRange()
    {
        while (true)
        {
            Collider2D[] hitAliens = Physics2D.OverlapCircleAll(transform.position, attackRange);
            foreach (Collider2D alienCollider in hitAliens)
            {
                if (alienCollider.CompareTag("Alien"))
                {
                    var alien = alienCollider.GetComponent<Alien>();
                    if (alien != null)
                    {
                        alien.LoseHealthServerRpc(attackPower);
                        Debug.Log($"Attacking alien: {alienCollider.name}");
                    }
                }
            }
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
