using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class DittoAlien : Alien
{
    public int slimePerInterval = 5;
    public float slimeProductionInterval = 10f;
    public GameObject slimePrefab;
    public float slimeLifetime = 2f;

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
            yield return new WaitForSeconds(slimeProductionInterval);
            ProduceSlimeServerRpc(slimePerInterval);
            Debug.Log("produced slime");
        }
    }

    [ServerRpc]
    private void ProduceSlimeServerRpc(int amount)
    {
        Vector3 spawnOffset = new Vector3(-0.10f, 0.33f, 0f);
        Vector3 spawnPosition = transform.position + spawnOffset;
        GameObject slime = Instantiate(slimePrefab, spawnPosition, Quaternion.identity);
        slime.GetComponent<NetworkObject>().Spawn();
        CurrencyManager.instance.AddAlienCurrency(amount);
        Debug.Log("produced slime serverRpC: " + slime);
        Destroy(slime, slimeLifetime);

        ProduceSlimeClientRpc(amount);
    }

    [ClientRpc]
    void ProduceSlimeClientRpc(int amount)
    {
        
    }

}
