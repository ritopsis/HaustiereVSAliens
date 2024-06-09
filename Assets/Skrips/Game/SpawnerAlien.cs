using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerAlien : MonoBehaviour
{
    public List<GameObject> aliensPrefabs;
    public Transform spawnAlienRoot;
    public List<UnityEngine.UI.Image> aliensUI;
    int spawnID = -1;
    private List<Transform> spawnPoints;

    void Start()
    {
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("AlienSpawn");
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
                SpawnAlien(selectedSpawnPoint.position, selectedSpawnPoint);
                spawnPoints.Remove(selectedSpawnPoint);
                selectedSpawnPoint.gameObject.SetActive(false);
            }
        }
    }

    void SpawnAlien(Vector3 position, Transform spawnPoint)
    {
        GameObject alien = Instantiate(aliensPrefabs[spawnID], spawnAlienRoot);
        alien.transform.position = position;
        alien.GetComponent<Alien>().Init(spawnPoint);

        DeselectAliens();
    }

    void DeselectAliens()
    {
        spawnID = -1;
        foreach (var alienUI in aliensUI)
        {
            alienUI.color = Color.white;
        }
    }

    public void RevertSpawnPoint(Transform spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
        spawnPoint.gameObject.SetActive(true);
    }

    public void SelectAlien(int id)
    {
        if (id >= 0 && id < aliensPrefabs.Count)
        {
            spawnID = id;
            for (int i = 0; i < aliensUI.Count; i++)
            {
                if (i == id)
                {
                    aliensUI[i].color = Color.green;
                }
                else
                {
                    aliensUI[i].color = Color.white;
                }
            }
        }
    }
}
