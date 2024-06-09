using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SpawnerAlien : MonoBehaviour
{
   // list of pets (prefabs) that will instantiate
    public List<GameObject> alienPrefabs;

    public Transform spawnAlienRoot;

    // list of pets UI
    public List<UnityEngine.UI.Image> alienUI;

    // id of pet to spawn (-1 means none)
    int spawnID = -1;

    //Spawn Area for pets Tilemap
    public Tilemap AlienspawnTilemap;

    // Tile position
    private Vector3Int tilePos;

    void Update(){
        if (CanSpawn()){
           DetectSpawnPoint(); 
        }
        
    }

    bool CanSpawn()
    {
        if (spawnID == -1)
            return false;
        else
            return true;
    }

    void DetectSpawnPoint()
    {

        if(Input.GetMouseButtonDown(0))
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var cellPosDefault = AlienspawnTilemap.WorldToCell(mousePos);

            var cellPosCentered = AlienspawnTilemap.GetCellCenterWorld(cellPosDefault);

            if(AlienspawnTilemap.GetColliderType(cellPosDefault)==Tile.ColliderType.Sprite)
            {
                //towercost

                SpawnAlien(cellPosCentered, cellPosDefault);
                
//AlienspawnTilemap.SetColliderType(cellPosDefault, Tile.ColliderType.None);

                
            }


            //var cellPosRaw = PetspawnTilemap.LocalToWorld(mousePos);
            //Debug.Log(cellPosRaw);
        }


    }   

    

    void SpawnAlien(Vector3 position, Vector3Int cellPosition)
    {
        GameObject alien = Instantiate(alienPrefabs[spawnID],spawnAlienRoot);
        alien.transform.position = position;
        alien.GetComponent<Alien>().Init(cellPosition);

        DeselectAliens();
    }

    /*void SpawnPet(Vector3 position, Vector3Int cellPosition)
{
    if (petsPrefabs == null || spawnID < 0 || spawnID >= petsPrefabs.Count)
    {
        Debug.LogError("Invalid spawnID or petsPrefabs list is null");
        return;
    }

    if (spawnPetRoot == null)
    {
        Debug.LogError("spawnPetRoot has not been assigned in the Inspector");
        return;
    }

    GameObject pet = Instantiate(petsPrefabs[spawnID], spawnPetRoot);
    if (pet == null)
    {
        Debug.LogError("Failed to instantiate pet");
        return;
    }

    Pet petComponent = pet.GetComponent<Pet>();
    if (petComponent == null)
    {
        Debug.LogError("Pet component not found on the instantiated object");
        return;
    }

    petComponent.Init(cellPosition);
    DeselectPets();
}
*/
        
    public void RevertCellState(Vector3Int pos){
        AlienspawnTilemap.SetColliderType(pos, Tile.ColliderType.Sprite);
    }
    public void SelectAlien (int id)
    {
        DeselectAliens();

        spawnID = id;

        alienUI[spawnID].color = Color.white;
    }

    public void DeselectAliens()
    {
        spawnID = -1;
        foreach(var t in alienUI)
        {
            t.color = new Color(0.5f, 0.5f, 0.5f);
        }
    }
}
