using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class SpawnManager : MonoBehaviour
{
    Collider[] spawnAreas; // The areas where objects will be instantiated.
    public GameObject[] platformPrefabs;
    public int levelCounter;
    public GameObject pickupObject;
    private List<GameObject> pickupPool = new List<GameObject>();
    public int initialPoolSize = 100;

    public GameObject playerPrefab;
    public GameObject npcPrefab;
    public CameraFollow mainCamera;

    public float spawnIntervalTime = 0.2f; // PickUp Spawn Time
    public int enemyCount = 11; // Total Enemy Count
    
    GameManager gameManager;

    // Nicknames for AI's
    string[] aiNames = {
    "Miss Mustard",
    "Cotton",
    "Amigo",
    "Drop Stone",
    "Blood Taker",
    "Willow Wisp",
    "Rusty Blade",
    "Ember Whisper",
    "Shadow Dancer",
    "Luna Frost",
    "Grimshaw",
    "Sapphire",
    "Whispering Oak",
    "Steelheart",
    "Willowbreeze",
    "Blaze Fang",
    "Nightshade",
    "Ashen Thorn",
    "Raven Shadow",
    "Frostbite",
    "Ahmet Meric",
    "Murat Yucel",
    "Samet Meric"
    };

    GameObject SetLevel()
    {
        levelCounter = PlayerPrefs.GetInt("levelCounter", 0);

        if (levelCounter >= platformPrefabs.Length)
        {
            levelCounter = 0;
        }

        GameObject currentPlatform = Instantiate(platformPrefabs[levelCounter], Vector3.zero, Quaternion.identity, transform);
        levelCounter++;
        PlayerPrefs.SetInt("levelCounter", levelCounter);

        return currentPlatform;
    }
    void Start()
    {
        spawnAreas = SetLevel().GetComponentsInChildren<Collider>();
        gameManager = GameManager.Instance;

        SetPickupObjectMesh();
        InitializePool();
        StartCoroutine(SpawnPickups());
        SpawnCharacters();
    }

    private void SetPickupObjectMesh()
    {
        List<MarketItem> currentItems = MarketManager.Instance.GetItemsForGame();   
        pickupObject.transform.GetChild(0).GetComponent<MeshFilter>().mesh =
            currentItems[0].GetComponent<MarketItem>().itemPrefab.GetComponent<MeshFilter>().sharedMesh;
    }

    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject pickup = Instantiate(pickupObject, transform.position, Quaternion.identity);
            pickup.SetActive(false);
            pickupPool.Add(pickup);
        }
    }

    // Instantiate the object continuously at certain intervals.
    IEnumerator SpawnPickups()
    {
        yield return new WaitForSeconds(spawnIntervalTime);
        // Get an inactive pick-up object from the pool using it.
        GameObject pickup = GetInactivePickup();

        if (pickup != null)
        {
            pickup.transform.position = GetRandomPoint();
            pickup.SetActive(true);
        }

        StartCoroutine(SpawnPickups());
    }

    private GameObject GetInactivePickup()
    {
        foreach (GameObject pickup in pickupPool)
        {
            if (!pickup.activeInHierarchy)
            {
                return pickup;
            }
        }

        // If no inactive pick-up object is found, a new object is created and added to the pool.
        GameObject newPickup = Instantiate(pickupObject, transform.position, Quaternion.identity);
        newPickup.SetActive(false);
        pickupPool.Add(newPickup);

        return newPickup;
    }

    // Spawn Player and NPC's.
    public void SpawnCharacters()
    {
        List<Vector3> spawnPoints = GetSpawnPoints(enemyCount + 1, 15f);

        GameObject player = Instantiate(playerPrefab, spawnPoints[0], Quaternion.identity);
        Destroy(playerPrefab);
        gameManager.livingCharacters.Add(player.GetComponent<CharacterFeatures>());
        SetPlayer(player.GetComponent<CharacterFeatures>());
        //player.GetComponent<CharacterFeatures>().SetName(gameManager.nameInput.text);
        //gameManager.SavePlayerName();
        //Camera.main.GetComponent<CameraFollow>().target = player.transform;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(npcPrefab, spawnPoints[i + 1], Quaternion.identity);
            enemy.GetComponent<CharacterFeatures>().SetName(aiNames[Random.Range(0, aiNames.Length)]);
            gameManager.livingCharacters.Add(enemy.GetComponent<CharacterFeatures>());
        }
    }

    public void SetPlayer(CharacterFeatures player)
    {
        player.GetComponent<CharacterFeatures>().SetName(gameManager.nameInput.text);
        player.GetComponent<Rigidbody>().isKinematic = false;
        gameManager.SavePlayerName();
       mainCamera.target = player.transform;
    }
   

    //When the game starts, creates a Vector3 List with a minDistance
    //condition to prevent all players from Instantiate too close to each other.
    private List<Vector3> GetSpawnPoints(int pointCount, float minDistance)
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        //Collider selectedArea = spawnAreas[Random.Range(0, spawnAreas.Length)];

        //Vector3 center = spawnArea.bounds.center;
        //Vector3 size = spawnArea.bounds.size;

        //float halfWidth = size.x / 2f;
        //float halfDepth = size.z / 2f;

        while (spawnPoints.Count < pointCount)
        {
            Collider selectedArea = spawnAreas[Random.Range(0, spawnAreas.Length)];

            Vector3 center = selectedArea.bounds.center;
            Vector3 size = selectedArea.bounds.size;

            float halfWidth = size.x / 2f;
            float halfDepth = size.z / 2f;

            Vector3 randomPoint = center + new Vector3(Random.Range(-halfWidth, halfWidth), 
                0f, Random.Range(-halfDepth, halfDepth));

            bool validPoint = true;

            foreach (Vector3 point in spawnPoints)
            {
                if (Vector3.Distance(randomPoint, point) < minDistance)
                {
                    validPoint = false;
                    break;
                }
            }

            if (validPoint)
            {
                spawnPoints.Add(randomPoint + Vector3.up);
            }
        }

        return spawnPoints;
    }

    // Return a random point within the collider's bounded area. For Pickup Objects
    public Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = Vector3.zero;

        Collider selectedArea = spawnAreas[Random.Range(0, spawnAreas.Length)];

        if (selectedArea != null)
        {
            //Bounds bounds = spawnArea.bounds;
            Bounds bounds = selectedArea.bounds;

            randomPoint = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
        else
        {
            Debug.LogWarning("Collider reference is missing!");
        }

        return randomPoint + new Vector3(0, 10, 0);
    }
}
