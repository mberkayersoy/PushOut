using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public Collider[] spawnAreas;
    void Start()
    {
        
    }

    public void GetColliders()
    {
        spawnAreas = GetComponentsInChildren<Collider>();
    }
}
