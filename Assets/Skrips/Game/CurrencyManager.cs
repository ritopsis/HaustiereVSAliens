using UnityEngine;
using TMPro;
using Unity.Netcode;

public class CurrencyManager : NetworkBehaviour
{
    public static CurrencyManager instance;

    //private NetworkVariable<int> petCurrency = new NetworkVariable<int>();
    //private NetworkVariable<int> alienCurrency = new NetworkVariable<int>();

    private int petCurrency = 0;
    private int alienCurrency = 0;
    public TMP_Text petCurrencyText; // Reference to the TextMeshPro component for pet currency
    public TMP_Text alienCurrencyText; // Reference to the TextMeshPro component for alien currency

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
        
    }

    
    public void AddPetCurrency(int amount)
    {
       
        //if (IsServer)
        //{
            petCurrency+= amount;
            UpdateCurrencyUI();
            Debug.Log("Add pet currency");
        //}
    }

    public void AddAlienCurrency(int amount)
    {
        
        //if (IsServer)
        //{
            alienCurrency += amount;
            UpdateCurrencyUI();
        //}
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
