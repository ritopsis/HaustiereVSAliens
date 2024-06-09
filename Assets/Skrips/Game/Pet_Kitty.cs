using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Pet_Kitty : Pet
{




    public int incomeValue;

    public float interval;

    public GameObject obj_sun;
    // Start is called before the first frame update
    protected override void Start()
    {
        Debug.Log("Kitty");
        if (obj_sun == null)
        {
            Debug.LogError("obj_sun is not assigned!");
        }
        StartCoroutine(Interval());
    }
    IEnumerator Interval()
    {
        Debug.Log("Starting income interval");
        yield return new WaitForSeconds(interval);
        IncreaseIncome();
        StartCoroutine(Interval());
    }

    public void IncreaseIncome()
    {
        //GameManager.instance.currency.Gain(incomeValue);
        StartCoroutine(CoinIndication());


    }

    IEnumerator CoinIndication()
    {
        obj_sun.SetActive(true);
        yield return new WaitForSeconds (0.5f);
        obj_sun.SetActive(false);
    }

    
}

