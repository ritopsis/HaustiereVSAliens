using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class CurrencyManager : NetworkBehaviour
{
    public static CurrencyManager instance;

    //private NetworkVariable<int> petCurrency = new NetworkVariable<int>();
    //private NetworkVariable<int> alienCurrency = new NetworkVariable<int>();

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

    void Start()
    {
        petCurrencyText.text = petCurrency.ToString();
        alienCurrencyText.text = alienCurrency.ToString();

    }

    
    public void AddPetCurrency(int amount)
    {
       
        //if (IsServer)
        //{
            petCurrency+= amount;
            UpdateCurrencyUI();
            OnCurrencyChanged();
            Debug.Log("Add pet currency");
        //}
    }

    public void AddAlienCurrency(int amount)
    {
        
        //if (IsServer)
        //{
            alienCurrency += amount;
            UpdateCurrencyUI();
            OnCurrencyChanged();
        //}
    }

    public void SubtractPetCurrency(int amount) 
    {
        petCurrency -= amount; 
        UpdateCurrencyUI();
        OnCurrencyChanged();
    }

    public void SubtractAlienCurrency(int amount)
    {
        alienCurrency -= amount;
        UpdateCurrencyUI();
        OnCurrencyChanged();
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
