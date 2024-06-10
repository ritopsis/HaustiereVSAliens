using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Pet_Kitty : Pet
{
    public int sunsPerInterval = 5;
    public float sunProductionInterval = 10f;
    public GameObject sunPrefab;
    public float sunLifetime = 2f;


    protected override void Start()
    {
        base.Start();
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(ProduceCurrency());
        }
    }

    IEnumerator ProduceCurrency()
    {
        while (true)
        {
            yield return new WaitForSeconds(sunProductionInterval);
            ProduceSunServerRpc(sunsPerInterval);
            Debug.Log("produced sun");
        }
    }

    [ServerRpc]
    void ProduceSunServerRpc(int amount)
    {
        Vector3 spawnOffset = new Vector3(-0.10f, 0.27f, 0f);
        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject sun = Instantiate(sunPrefab, spawnPosition, Quaternion.identity);
        sun.GetComponent<NetworkObject>().Spawn();
        CurrencyManager.instance.AddPetCurrency(amount);
        Debug.Log("produced sun serverRpC: " + sun);
        Destroy(sun, sunLifetime);

        ProduceSunClientRpc(amount);
        
    }

    [ClientRpc]
    void ProduceSunClientRpc(int amount)
    {
        CurrencyManager.instance.AddPetCurrency(amount);
    }


}
