using UnityEngine;
using TMPro;
using Unity.Netcode;

public class CurrencyManager : NetworkBehaviour
{
    public static CurrencyManager instance;

    private NetworkVariable<int> petCurrency = new NetworkVariable<int>();
    private NetworkVariable<int> alienCurrency = new NetworkVariable<int>();

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
        petCurrency.OnValueChanged += OnPetCurrencyChanged;
        alienCurrency.OnValueChanged += OnAlienCurrencyChanged;
        UpdateCurrencyUI();
    }

    public void AddPetCurrency(int amount)
    {
        if (IsServer)
        {
            petCurrency.Value += amount;
        }
    }

    public void AddAlienCurrency(int amount)
    {
        if (IsServer)
        {
            alienCurrency.Value += amount;
        }
    }

    public int GetPetCurrency()
    {
        return petCurrency.Value;
    }

    public int GetAlienCurrency()
    {
        return alienCurrency.Value;
    }

    private void OnPetCurrencyChanged(int oldValue, int newValue)
    {
        UpdateCurrencyUI();
    }

    private void OnAlienCurrencyChanged(int oldValue, int newValue)
    {
        UpdateCurrencyUI();
    }

    private void UpdateCurrencyUI()
    {
        if (petCurrencyText != null)
        {
            petCurrencyText.text = "Pet Currency: " + petCurrency.Value;
        }

        if (alienCurrencyText != null)
        {
            alienCurrencyText.text = "Alien Currency: " + alienCurrency.Value;
        }

        Debug.Log("Pet Currency: " + petCurrency.Value + " | Alien Currency: " + alienCurrency.Value);
    }
}
