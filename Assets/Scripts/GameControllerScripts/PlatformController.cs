using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using System.Linq;

public class PlatformController : MonoBehaviour
{
    public List<Transform> grids;
    NavMeshSurface area;
    [SerializeField] private float fallTime;
    float remainingTime;
    bool isStart;

    void Start()
    {
        grids = GetComponentsInChildren<Transform>().ToList();
        grids.Remove(gameObject.transform);
        remainingTime = fallTime;
        area = GetComponent<NavMeshSurface>();
    }


    IEnumerator UpdateArea()
    {
        isStart = true; 
        int randomIndex = Random.Range(0, grids.Count);
        grids[randomIndex].DOShakePosition(3f, 1f, 1, 2).OnComplete(()=>
        {
            grids.Remove(grids[randomIndex]);
            Destroy(grids[randomIndex].gameObject);

        });
        area.BuildNavMesh();
        //grids.Remove(grids[randomIndex]);
        yield return new WaitForSeconds(4f);
        StartCoroutine(UpdateArea());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStart)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 0)
            {
                StartCoroutine(UpdateArea());
            }
        }
    }
}
