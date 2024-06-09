using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerPet : MonoBehaviour
{
    // List of pets (prefabs) that will instantiate
    public List<GameObject> petsPrefabs;

    public Transform spawnPetRoot;

    // List of pets UI
    public List<UnityEngine.UI.Image> petsUI;

    // ID of pet to spawn (-1 means none)
    int spawnID = -1;

    // List of spawn points (object containers)
    private List<Transform> spawnPoints;

    void Start()
    {
        // Find all spawn points tagged with "PetSpawn"
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
            mousePos.z = 0;  // Ensure Z coordinate is zero for 2D

            Transform selectedSpawnPoint = null;

            // Find the closest spawn point to the mouse position
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
                // Spawn the pet at the selected spawn point
                SpawnPet(selectedSpawnPoint.position, selectedSpawnPoint);
                
                // Optionally, you can disable the spawn point or make it unavailable for future spawns
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
            petUI.color = Color.white; // Or any other logic to reset UI selection
        }
    }

    public void RevertSpawnPoint(Transform spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
        spawnPoint.gameObject.SetActive(true);
    }

    // New method to select a pet based on user input
    public void SelectPet(int id)
    {
        if (id >= 0 && id < petsPrefabs.Count)
        {
            spawnID = id;
            // Highlight the selected pet UI
            for (int i = 0; i < petsUI.Count; i++)
            {
                if (i == id)
                {
                    petsUI[i].color = Color.green; // Or any other color to indicate selection
                }
                else
                {
                    petsUI[i].color = Color.white;
                }
            }
        }
    }
}
