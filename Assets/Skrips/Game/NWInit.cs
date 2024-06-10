using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class NetworkInitializer : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;

    void Start()
    {
        // already defined in test.cs

        //hostButton.onClick.AddListener(StartHost);
        //clientButton.onClick.AddListener(StartClient);
        //serverButton.onClick.AddListener(StartServer);
    }

    void StartHost()
    {
        Debug.Log("Starting Host...");
        NetworkManager.Singleton.StartHost();
    }

    void StartClient()
    {
        Debug.Log("Starting Client...");
        NetworkManager.Singleton.StartClient();
    }

    void StartServer()
    {
        Debug.Log("Starting Server...");
        NetworkManager.Singleton.StartServer();
    }
}
