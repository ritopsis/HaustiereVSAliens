using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class SpawnerAlien : NetworkBehaviour
{
    // List of aliens (prefabs) that will instantiate
    public List<GameObject> aliensPrefabs;

    public Transform spawnAlienRoot;

    // List of aliens UI
    public List<Image> aliensUI;

    // ID of alien to spawn (-1 means none)
    int spawnID = -1;

    // List of spawn points (object containers)
    private List<Transform> spawnPoints;
    private Dictionary<Transform, GameObject> dittoAliens = new Dictionary<Transform, GameObject>();

    void Start()
    {
        // Find all spawn points tagged with "AlienSpawn"
        GameObject[] spawnPointObjects = GameObject.FindGameObjectsWithTag("AlienSpawn");
        spawnPoints = new List<Transform>();
        foreach (var obj in spawnPointObjects)
        {
            spawnPoints.Add(obj.transform);
        }
    }

    void Update()
    {
        if (IsClient && CanSpawn())
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
                if (CanSpawnDitto(selectedSpawnPoint))
                {
                    RequestSpawnAlienServerRpc(spawnID, selectedSpawnPoint.position, selectedSpawnPoint.GetComponent<NetworkObject>().NetworkObjectId);
                    spawnPoints.Remove(selectedSpawnPoint);
                    selectedSpawnPoint.gameObject.SetActive(false);
                }
            }
        }
    }

    bool CanSpawnDitto(Transform spawnPoint)
    {
        if (aliensPrefabs[spawnID].name == "DittoAlien")
        {
            if (!dittoAliens.ContainsKey(spawnPoint) || dittoAliens[spawnPoint] == null)
            {
                dittoAliens[spawnPoint] = null; // Reserve the spot
                return true;
            }
            return false;
        }
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestSpawnAlienServerRpc(int id, Vector3 position, ulong spawnPointId)
    {
        SpawnAlienClientRpc(id, position, spawnPointId);
    }

    [ClientRpc]
    void SpawnAlienClientRpc(int id, Vector3 position, ulong spawnPointId)
    {
        GameObject alien = Instantiate(aliensPrefabs[id], spawnAlienRoot);
        alien.transform.position = position;
        alien.GetComponent<NetworkObject>().Spawn();

        Transform spawnPoint = NetworkManager.Singleton.SpawnManager.SpawnedObjects[spawnPointId].transform;
        alien.GetComponent<Alien>().Init(spawnPoint);

        if (aliensPrefabs[id].name == "DittoAlien")
        {
            dittoAliens[spawnPoint] = alien;
        }

        DeselectAliens();
    }

    void DeselectAliens()
    {
        spawnID = -1;
        foreach (var alienUI in aliensUI)
        {
            alienUI.color = Color.white; // Or any other logic to reset UI selection
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
            // Highlight the selected alien UI
            for (int i = 0; i < aliensUI.Count; i++)
            {
                if (i == id)
                {
                    aliensUI[i].color = Color.green; // Or any other color to indicate selection
                }
                else
                {
                    aliensUI[i].color = Color.white;
                }
            }
        }
    }

    public void NotifyDittoDeath(Transform spawnPoint)
    {
        if (dittoAliens.ContainsKey(spawnPoint))
        {
            dittoAliens[spawnPoint] = null;
        }
    }
}



/*
public class SpawnerAlien : MonoBehaviour
{
    // List of aliens (prefabs) that will instantiate
    public List<GameObject> aliensPrefabs;

    public Transform spawnAlienRoot;

    // List of aliens UI
    public List<UnityEngine.UI.Image> aliensUI;

    // ID of alien to spawn (-1 means none)
    int spawnID = -1;

    // List of spawn points (object containers)
    private List<Transform> spawnPoints;
    private Dictionary<Transform, GameObject> dittoAliens = new Dictionary<Transform, GameObject>();

    void Start()
    {
        // Find all spawn points tagged with "AlienSpawn"
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
                if (CanSpawnDitto(selectedSpawnPoint))
                {
                    SpawnAlien(selectedSpawnPoint.position, selectedSpawnPoint);
                }
            }
        }
    }

    bool CanSpawnDitto(Transform spawnPoint)
    {
        if (aliensPrefabs[spawnID].name == "DittoAlien")
        {
            if (!dittoAliens.ContainsKey(spawnPoint) || dittoAliens[spawnPoint] == null)
            {
                dittoAliens[spawnPoint] = null; // Reserve the spot
                return true;
            }
            return false;
        }
        return true;
    }

    void SpawnAlien(Vector3 position, Transform spawnPoint)
    {
        GameObject alien = Instantiate(aliensPrefabs[spawnID], spawnAlienRoot);
        alien.transform.position = position;
        alien.GetComponent<Alien>().Init(spawnPoint);

        if (aliensPrefabs[spawnID].name == "DittoAlien")
        {
            dittoAliens[spawnPoint] = alien;
        }

        DeselectAliens();
    }

    void DeselectAliens()
    {
        spawnID = -1;
        foreach (var alienUI in aliensUI)
        {
            alienUI.color = Color.white; // Or any other logic to reset UI selection
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
            // Highlight the selected alien UI
            for (int i = 0; i < aliensUI.Count; i++)
            {
                if (i == id)
                {
                    aliensUI[i].color = Color.green; // Or any other color to indicate selection
                }
                else
                {
                    aliensUI[i].color = Color.white;
                }
            }
        }
    }

    public void NotifyDittoDeath(Transform spawnPoint)
    {
        if (dittoAliens.ContainsKey(spawnPoint))
        {
            dittoAliens[spawnPoint] = null;
        }
    }
}
*/