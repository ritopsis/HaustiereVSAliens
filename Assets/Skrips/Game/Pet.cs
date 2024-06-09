using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pet : MonoBehaviour
{

    public int health;
    public int cost;
    private Vector3Int cellPosition;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        Debug.Log("Pet");
        
    }

    public virtual void Init(Vector3Int cellPos)
    {
        cellPosition =cellPos;

    }

    public virtual bool LoseHealth(int amount)
    {
        health = health - amount;
        if (health <=0){

            //Die()
            return true;
        }
        return false;
    
    }

    protected virtual void Die()
    {
        Debug.Log("Pet died");
        FindObjectOfType<SpawnerPet>().RevertCellState(cellPosition);
        Destroy(gameObject);
    }


}
