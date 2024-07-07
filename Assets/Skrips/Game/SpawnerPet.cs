using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class SpawnerPet : NetworkBehaviour
{

    public List<GameObject> petsPrefabs;
    public List<Pet> pets;
    public Transform spawnPetRoot;
    public List<Image> petsUI;
    int spawnID = -1;
    private List<Transform> spawnPoints;
    private string mouseclick;
    List<GameObject> listOfObject = new List<GameObject>();
    
    private Dictionary<Transform, GameObject> spawnedPets = new Dictionary<Transform, GameObject>();
    public Button removeButton; 
    public Image removeButtonImage; // Reference to the Remove button image
    private bool isRemoveMode = false;
    void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("PetSpawn");
        spawnPoints = new List<Transform>();
        foreach (var obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
        }

        CurrencyManager.instance.OnCurrencyChanged += UpdatePetCardsUI;

        // Set up the remove button
        removeButton.onClick.AddListener(() => SelectPet(-1));
    }

    void Update()
    {
        //if (IsClient && CanSpawn())
        if (IsClient && CanSpawnOrRemove())
        {
            DetectSpawnPointOrPet();
        }
    }

    bool CanSpawnOrRemove()
    {
        return spawnID != -1 || isRemoveMode;
    }

    bool CanSpawn()
    {
        //return spawnID != -1;
        return spawnID != -1 && !isRemoveMode;
    }

    void DetectSpawnPointOrPet()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUITag())
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                if (isRemoveMode && hit.collider.CompareTag("Pet"))
                {
                    Debug.Log("Pet detected for removal: " + hit.collider.gameObject.name);
                    RemovePet(hit.collider.gameObject);
                }
                else if (!isRemoveMode)
                {
                    DetectSpawnPoint(mousePos);
                }
            }
            else if (!isRemoveMode)
            {
                DetectSpawnPoint(mousePos);
            }
        }
    }

    void DetectSpawnPoint(Vector3 mousePos)
    {
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

        if (selectedSpawnPoint != null && selectedSpawnPoint.gameObject.activeInHierarchy && CanSpawn())
        {
            Pet pet = petsPrefabs[spawnID].GetComponent<Pet>();
            if (CurrencyManager.instance.GetPetCurrency() >= pet.cost)
            {
                RequestSpawnPetServerRpc(spawnID, selectedSpawnPoint.position, selectedSpawnPoint.GetComponent<NetworkObject>().NetworkObjectId);
            }
            else
            {
                Debug.Log("Not enough currency to spawn this pet which costs " + pet.cost + " and you have " + CurrencyManager.instance.GetPetCurrency());
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnPetServerRpc(int id, Vector3 position, ulong spawnPointId)
    {
        GameObject pet = Instantiate(petsPrefabs[id], position, Quaternion.identity, spawnPetRoot);
        var networkObject = pet.GetComponent<NetworkObject>();
        networkObject.Spawn();

        Pet petObject = pet.GetComponent<Pet>();
        CurrencyManager.instance.SubtractPetCurrency(petObject.cost);

        Transform spawnPoint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform;
        spawnedPets[spawnPoint] = pet;
        spawnPoint.gameObject.SetActive(false); // Mark the spawn point as occupied

        petObject.Init(spawnPoint);
        NotifySpawnPetClientRpc(pet.GetComponent<NetworkObject>().NetworkObjectId, spawnPointId);
        DeselectPets();
    }

    [ClientRpc]
    void NotifySpawnPetClientRpc(ulong petNetworkObjectId, ulong spawnPointId)
    {
        if (NetworkManager.Singleton.IsServer) return;

        Transform spawnPoint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform;
        spawnPoint.gameObject.SetActive(false); // Mark the spawn point as occupied

        DeselectPets();
    }

      void DeselectPets()
    {
        spawnID = -1;
        foreach (var petUI in petsUI)
        {
            petUI.color = Color.white;
        }

        if (removeButtonImage != null)
        {
            removeButtonImage.color = Color.white; // Reset the remove button color
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RevertSpawnPointServerRpc(ulong spawnPointId)
    {
        var spawnPoint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform;
        spawnPoint.gameObject.SetActive(true);
        RevertSpawnPointClientRpc(spawnPointId);
    }

    [ClientRpc]
    public void RevertSpawnPointClientRpc(ulong spawnPointId)
    {
        var spawnPoint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform;
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
        /*if (id >= 0 && id < petsPrefabs.Count)
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
        }*/

        spawnID = id;
        isRemoveMode = (id == -1);
        
        for (int i = 0; i < petsUI.Count; i++)
        {
            petsUI[i].color = (i == id) ? Color.green : Color.white;
        }

        if (removeButtonImage != null)
        {
            removeButtonImage.color = isRemoveMode ? Color.red : Color.white;
        }
    }
    
    public void WhatMouse(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.button == PointerEventData.InputButton.Left)
        {
            mouseclick = "Left";
        }
        else if (pointerData.button == PointerEventData.InputButton.Right)
        {
            mouseclick = "Right";
        }

    }
    public void Show(GameObject text)
    {
        if (mouseclick == "Right")
        {
            if(!listOfObject.Contains(text))
            {
                listOfObject.Add(text);
                Debug.Log("added" + text);
                text.SetActive(true);
                StartCoroutine(DeactivateAfterTime(text, 5.0f)); // Deactivate after 5 seconds
            }
        }
    }

    private IEnumerator DeactivateAfterTime(GameObject text, float delay)
    {
        yield return new WaitForSeconds(delay);
        text.SetActive(false);
        listOfObject.Remove(text);
    }


    public bool IsPointerOverUITag() // to check if a pet was selected or if the player is trying to place a pet
    {
        string tag = "PetSelect";
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        // Raycast on UI-Elemente 
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, raycastResults);

        // check if it has the tag
        foreach (var result in raycastResults)
        {
            if (result.gameObject.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }

    void RemovePet(GameObject pet)
    {
        var networkObject = pet.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            Debug.Log("Requesting server to remove pet: " + networkObject.NetworkObjectId);
            RequestRemovePetServerRpc(networkObject.NetworkObjectId);
        }
    }

    
    [ServerRpc(RequireOwnership = false)]
    void RequestRemovePetServerRpc(ulong petNetworkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(petNetworkObjectId, out var networkObject))
        {
            Transform spawnPoint = null;
            if (spawnedPets.ContainsValue(networkObject.gameObject))
            {
                foreach (var entry in spawnedPets)
                {
                    if (entry.Value == networkObject.gameObject)
                    {
                        spawnPoint = entry.Key;
                        break;
                    }
                }
            }

            if (spawnPoint != null)
            {
                Debug.Log("Server removing pet: " + networkObject.NetworkObjectId);
                Destroy(networkObject.gameObject);
                spawnedPets.Remove(spawnPoint);
                RevertSpawnPointServerRpc(spawnPoint.GetComponent<NetworkObject>().NetworkObjectId);
            }
        }
        else
        {
            Debug.Log("Server could not find pet: " + petNetworkObjectId);
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