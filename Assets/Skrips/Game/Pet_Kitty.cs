using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Pet_Kitty : Pet
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
            AddPetCurrencyServerRpc(currencyPerInterval);
        }
    }

    [ServerRpc]
    private void AddPetCurrencyServerRpc(int amount)
    {
        CurrencyManager.instance.AddPetCurrency(amount);
    }
}
