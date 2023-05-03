using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject spawnPointContainer;

    public float minTimeBetweenSpawns = 2;
    public float randomAdditionalTimeBetweenSpawns = 1;
    private List<Transform> spawnPointTransforms;
    private float timer = 0;
    private float nextSpawnTime;
    // Start is called before the first frame update
    private void Start()
    {
        spawnPointTransforms = new List<Transform>();
        nextSpawnTime = ChooseRandomTime();
        Debug.Log(spawnPointContainer.transform.childCount);
        foreach (Transform child in spawnPointContainer.transform)
        {
            spawnPointTransforms.Add(child);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (!(timer >= nextSpawnTime)) return;
        timer = 0;
        SpawnEnemy();
    }

    private float ChooseRandomTime()
    {
        return minTimeBetweenSpawns + Random.Range(0f, randomAdditionalTimeBetweenSpawns);
    }

    private void SpawnEnemy()
    {
        var spawnIndex = Random.Range(0, spawnPointTransforms.Count);
        Instantiate(enemyPrefab, spawnPointTransforms[spawnIndex]);
    }
}
