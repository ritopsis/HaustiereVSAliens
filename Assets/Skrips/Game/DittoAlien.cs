using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class DittoAlien : Alien
{
    public int currencyPerInterval = 5;
    public float productionInterval = 5f;

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
            yield return new WaitForSeconds(productionInterval);
            AddAlienCurrencyServerRpc(currencyPerInterval);
        }
    }

    [ServerRpc]
    private void AddAlienCurrencyServerRpc(int amount)
    {
        CurrencyManager.instance.AddAlienCurrency(amount);
    }
}
