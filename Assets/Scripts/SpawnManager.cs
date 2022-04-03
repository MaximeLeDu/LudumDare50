using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public List<MoveLeft> enemies;

    private float spawnMin = 1f;
    private float spawnMax = 3f;

    private float beginSpawTime = 5f;

    public bool isSpawning = true;

    private Vector3 pos = new Vector3(11, -2.7f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(beginSpawTime);
        while (isSpawning)
        {
            int enemiesIndex = Random.Range(0, enemies.Count);
            Instantiate(enemies[enemiesIndex], pos, Quaternion.identity);
            float wait = Random.Range(spawnMin, spawnMax);
            yield return new WaitForSeconds(wait);
        }
    }
}
