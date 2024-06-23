using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;
using System.Collections;

public class CurrencyManager : NetworkBehaviour
{
    public static CurrencyManager instance;

    public int petCurrency = 50;
    public int alienCurrency = 50;
    public TMP_Text petCurrencyText; // Reference to the TextMeshPro component for pet currency
    public TMP_Text alienCurrencyText; // Reference to the TextMeshPro component for alien currency

    public event Action OnCurrencyChanged = delegate { };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    

    
    private IEnumerator AddCurrencyEvery10Seconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Wait for 10 seconds

            Debug.Log("Added 5 currency");
            AddPetCurrency(5);
            AddAlienCurrency(5);

            UpdateCurrencyClientRpc(petCurrency, alienCurrency);
        }
    }



    void Start()
    {
        petCurrencyText.text = petCurrency.ToString();
        alienCurrencyText.text = alienCurrency.ToString();

        
        StartCoroutine(AddCurrencyEvery10Seconds());
        
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPetCurrencyServerRpc(int amount)
    {
        petCurrency += amount;
        UpdateCurrencyUI();
        OnCurrencyChanged();
        Debug.Log("Add pet currency");

        // Synchronize with clients
        UpdateCurrencyClientRpc(petCurrency, alienCurrency);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddAlienCurrencyServerRpc(int amount)
    {
        alienCurrency += amount;
        UpdateCurrencyUI();
        OnCurrencyChanged();

        // Synchronize with clients
        UpdateCurrencyClientRpc(petCurrency, alienCurrency);
    }

    [ClientRpc]
    private void UpdateCurrencyClientRpc(int newPetCurrency, int newAlienCurrency)
    {
        petCurrency = newPetCurrency;
        alienCurrency = newAlienCurrency;
        UpdateCurrencyUI();
    }

    public void AddPetCurrency(int amount)
    {
        if (IsServer)
        {
            AddPetCurrencyServerRpc(amount);
            Debug.Log("add pet currency");
        }
    }

    public void AddAlienCurrency(int amount)
    {
        if (IsServer)
        {
            AddAlienCurrencyServerRpc(amount);
        }
    }

    public void SubtractPetCurrency(int amount)
    {
        SubtractPetCurrencyServerRpc(amount);
    }


    public void SubtractAlienCurrency(int amount)
    {
        SubtractAlienCurrencyServerRpc(amount);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SubtractPetCurrencyServerRpc(int amount)
    {
        petCurrency -= amount;
        UpdateCurrencyUI();
        OnCurrencyChanged();

        // Synchronize with clients
        UpdateCurrencyClientRpc(petCurrency, alienCurrency);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubtractAlienCurrencyServerRpc(int amount)
    {
        alienCurrency -= amount;
        UpdateCurrencyUI();
        OnCurrencyChanged();

        // Synchronize with clients
        UpdateCurrencyClientRpc(petCurrency, alienCurrency);
    }

    public int GetPetCurrency()
    {
        return petCurrency;
    }

    public int GetAlienCurrency()
    {
        return alienCurrency;
    }

    private void UpdateCurrencyUI()
    {
        if (petCurrencyText != null)
        {
            petCurrencyText.text = petCurrency.ToString();
        }

        if (alienCurrencyText != null)
        {
            alienCurrencyText.text = alienCurrency.ToString();
        }

        Debug.Log("Pet Currency: " + petCurrency + " | Alien Currency: " + alienCurrency);
    }
}
