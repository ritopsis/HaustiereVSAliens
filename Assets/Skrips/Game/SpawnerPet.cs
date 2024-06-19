using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SpawnerPet : NetworkBehaviour
{
    public List<GameObject> petsPrefabs;
    public List<Pet> pets;
    public Transform spawnPetRoot;
    public List<Image> petsUI;
    int spawnID = -1;
    private List<Transform> spawnPoints;

    void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("PetSpawn");
        spawnPoints = new List<Transform>();
        foreach (var obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
        }
        Debug.Log("CurrencyManager.instance: " + CurrencyManager.instance == null);
        CurrencyManager.instance.OnCurrencyChanged += UpdatePetCardsUI;

    }

    void Update()
    {
        if (IsServer && CanSpawn())
        {
            DetectSpawnPoint();
        }
    }

    bool CanSpawn()
    {
        return spawnID != -1;
    }

    void DetectSpawnPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Transform selectedSpawnPoint = null;
            float minDistance = float.MaxValue;
            foreach (var spawnPoint in spawnPoints)
            {
                float distance = Vector3.Distance(mousePos, spawnPoint.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedSpawnPoint = spawnPoint;
                }
            }

            if (selectedSpawnPoint != null && CanSpawn())
            {
                Pet pet = petsPrefabs[spawnID].GetComponent<Pet>();
                if (CurrencyManager.instance.GetPetCurrency() >= pet.cost)
                {
                    RequestSpawnPetServerRpc(spawnID, selectedSpawnPoint.position, selectedSpawnPoint.GetComponent<NetworkObject>().NetworkObjectId);
                    spawnPoints.Remove(selectedSpawnPoint);
                    selectedSpawnPoint.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Not enough currency to spawn this pet which costs " + pet.cost + " and you have " + CurrencyManager.instance.GetPetCurrency());
                }
              
            }
        }
    }

    [ServerRpc]
    void RequestSpawnPetServerRpc(int id, Vector3 position, ulong spawnPointId)
    {
        GameObject pet = Instantiate(petsPrefabs[id], position, Quaternion.identity, spawnPetRoot);
        var networkObject = pet.GetComponent<NetworkObject>();
        networkObject.Spawn();

        Pet petObject = pet.GetComponent<Pet>();
        CurrencyManager.instance.SubtractPetCurrency(petObject.cost);
        petObject.Init(NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform);
        //petObject.OnDeath += HandlePetDeath; // Subscribe to the death event

        NotifySpawnPetClientRpc(pet.GetComponent<NetworkObject>().NetworkObjectId, spawnPointId);
        DeselectPets();
    }

    [ClientRpc]
    void NotifySpawnPetClientRpc(ulong petNetworkObjectId, ulong spawnPointId)
    {
        if (NetworkManager.Singleton.IsServer) return;

        var pet = NetworkManager.Singleton.SpawnManager.SpawnedObjects[petNetworkObjectId].GetComponent<Pet>();
        var spawnPoint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform;
        pet.Init(spawnPoint);
        DeselectPets();

    }

    void DeselectPets()
    {
        spawnID = -1;
        foreach (var petUI in petsUI)
        {
            petUI.color = Color.white;
        }
    }

    public void RevertSpawnPoint(Transform spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
        spawnPoint.gameObject.SetActive(true);
    }

    //private void HandlePetDeath(Pet pet)
    //{
    //    Transform spawnPoint = pet.transform.parent;
    //    spawnPoints.Add(spawnPoint);
    //    spawnPoint.gameObject.SetActive(true);
    //    pet.OnDeath -= HandlePetDeath; // Unsubscribe from the death event
    //    Debug.Log("handle pet death");
    //}

    public void UpdatePetCardsUI()
    {
        for(int i = 0; i < petsUI.Count; i++)
        {
            Pet pet = petsPrefabs[i].GetComponent<Pet>();
            if (CurrencyManager.instance.GetPetCurrency() < pet.cost)
            {
                petsUI[i].color = Color.grey;
            }
        }
    }

    public void SelectPet(int id)
    {
        if (id >= 0 && id < petsPrefabs.Count)
        {
            
            spawnID = id;
            for (int i = 0; i < petsUI.Count; i++)
            {
                if (i == id)
                {
                    petsUI[i].color = Color.green;
                }
                else
                {
                    petsUI[i].color = Color.white;
                }
            }
            
            
            
        }
    }


}






/* 

public class SpawnerPet : MonoBehaviour
{
    public List<GameObject> petsPrefabs;
    public Transform spawnPetRoot;
    public List<UnityEngine.UI.Image> petsUI;
    int spawnID = -1;
    private List<Transform> spawnPoints;

    void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("PetSpawn");
        spawnPoints = new List<Transform>();
        foreach (var obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
        }
    }

    void Update()
    {
        if (CanSpawn())
        {
            DetectSpawnPoint(); 
        }
    }

    bool CanSpawn()
    {
        return spawnID != -1;
    }

    void DetectSpawnPoint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            Transform selectedSpawnPoint = null;
            float minDistance = float.MaxValue;
            foreach (var spawnPoint in spawnPoints)
            {
                float distance = Vector3.Distance(mousePos, spawnPoint.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedSpawnPoint = spawnPoint;
                }
            }

            if (selectedSpawnPoint != null)
            {
                SpawnPet(selectedSpawnPoint.position, selectedSpawnPoint);
                spawnPoints.Remove(selectedSpawnPoint);
                selectedSpawnPoint.gameObject.SetActive(false);
            }
        }
    }

    void SpawnPet(Vector3 position, Transform spawnPoint)
    {
        GameObject pet = Instantiate(petsPrefabs[spawnID], spawnPetRoot);
        pet.transform.position = position;
        pet.GetComponent<Pet>().Init(spawnPoint);

        DeselectPets();
    }

    void DeselectPets()
    {
        spawnID = -1;
        foreach (var petUI in petsUI)
        {
            petUI.color = Color.white;
        }
    }

    public void RevertSpawnPoint(Transform spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
        spawnPoint.gameObject.SetActive(true);
    }

    public void SelectPet(int id)
    {
        if (id >= 0 && id < petsPrefabs.Count)
        {
            spawnID = id;
            for (int i = 0; i < petsUI.Count; i++)
            {
                if (i == id)
                {
                    petsUI[i].color = Color.green;
                }
                else
                {
                    petsUI[i].color = Color.white;
                }
            }
        }
    }
}
*/
