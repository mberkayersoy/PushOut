using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Collider spawnArea; // The area where objects will be instantiated.
    public GameObject pickupObject;
    public GameObject playerPrefab;
    public GameObject npcPrefab;
    public float spawnIntervalTime = 0.2f; // PickUp Spawn Time
    public int enemyCount = 11; // Total Enemy Count

    GameManager gameManager;

    // Nicknames for NPC's
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
    "Frostbite"
    };

    void Start()
    {
        spawnArea = GetComponent<Collider>();
        gameManager = GameManager.Instance;
        StartCoroutine(SpawnPickups());
        SpawnCharacters();
    }

    // Spawn Player and NPC's.
    public void SpawnCharacters()
    {
        List<Vector3> spawnPoints = GetSpawnPoints(enemyCount + 1, 15f);
        GameObject player = Instantiate(playerPrefab, spawnPoints[0], Quaternion.identity);
        gameManager.livingCharacters.Add(player.GetComponent<CharacterFeatures>());
        player.GetComponent<CharacterFeatures>().SetName(gameManager.nameInput.text);
        Camera.main.GetComponent<CameraFollow>().target = player.transform;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemy = Instantiate(npcPrefab, spawnPoints[i + 1], Quaternion.identity);
            enemy.GetComponent<CharacterFeatures>().SetName(aiNames[Random.Range(0, aiNames.Length)]);
            gameManager.livingCharacters.Add(enemy.GetComponent<CharacterFeatures>());
        }
    }

    // Instantiate the object continuously at certain intervals.
    IEnumerator SpawnPickups()
    {
        yield return new WaitForSeconds(spawnIntervalTime);
        Instantiate(pickupObject, GetRandomPoint(), Quaternion.identity);
        StartCoroutine(SpawnPickups());
    }

    //When the game starts, creates a Vector3 List with a minDistance
    //condition to prevent all players from Instantiate too close to each other.
    private List<Vector3> GetSpawnPoints(int pointCount, float minDistance)
    {
        List<Vector3> spawnPoints = new List<Vector3>();

        Vector3 center = spawnArea.bounds.center;
        Vector3 size = spawnArea.bounds.size;

        float halfWidth = size.x / 2f;
        float halfDepth = size.z / 2f;

        while (spawnPoints.Count < pointCount)
        {
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
                spawnPoints.Add(randomPoint);
            }
        }

        return spawnPoints;
    }

    // Return a random point within the collider's bounded area. For Pickup Objects
    public Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = Vector3.zero;

        if (spawnArea != null)
        {
            Bounds bounds = spawnArea.bounds;

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

        return randomPoint;
    }


}