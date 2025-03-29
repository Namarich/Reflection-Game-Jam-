using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    public int wave;
    public int startEnemyNumber;
    public int enemyNumberProgression;
    private int currentEnemiesSpawned;



    public GameObject enemy;

    public float timeBetweenSpawns;
    private float lastTimeSpawned;

    public float spawnZoneWidth;
    public float spawnZoneHeight;

    private float currentSpawnZoneWidth;
    private float currentSpawnZoneHeight;


    public List<GameObject> zones;
   


    // Start is called before the first frame update
    void Start()
    {
        lastTimeSpawned = 0;
        wave = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeBetweenSpawns + lastTimeSpawned && currentEnemiesSpawned < startEnemyNumber+(wave-1)*enemyNumberProgression)
        {
            Spawn(enemy);
            lastTimeSpawned = Time.time;
            currentEnemiesSpawned += 1;
        }
        else if (currentEnemiesSpawned >= startEnemyNumber + (wave - 1) * enemyNumberProgression)
        {
            StartCoroutine(NextWave());
        }
        Debug.Log($"wave:{wave}");
    }


    public void Spawn(GameObject enemy)
    {
        currentSpawnZoneWidth = spawnZoneWidth - enemy.GetComponent<SpriteRenderer>().bounds.extents.x;
        currentSpawnZoneHeight = spawnZoneHeight - enemy.GetComponent<SpriteRenderer>().bounds.extents.y;
        Vector2 spawnPos = new Vector2(Random.Range(currentSpawnZoneWidth*-1, currentSpawnZoneWidth), Random.Range(currentSpawnZoneHeight * -1, currentSpawnZoneHeight));
        int i = 0;
        while (true)
        {
            if (zones[i].GetComponent<SpriteRenderer>().bounds.Contains(spawnPos))
            {
                break;
            }
            else
            {
                i++;
                if (i == zones.Count)
                {
                    i = 0;
                }
            }
            spawnPos = new Vector2(Random.Range(currentSpawnZoneWidth * -1, currentSpawnZoneWidth), Random.Range(currentSpawnZoneHeight * -1, currentSpawnZoneHeight));
        }

        GameObject a = Instantiate(enemy,spawnPos,Quaternion.identity);
    }


    IEnumerator NextWave()
    {
        currentEnemiesSpawned = 0;
        wave += 1;
        yield return new WaitForSeconds(5f);
        lastTimeSpawned = 0;
    }
}
