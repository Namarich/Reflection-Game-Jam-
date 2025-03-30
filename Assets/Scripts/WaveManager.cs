using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{

    private Player player;

    public int wave;
    public int startEnemyNumber;
    public int enemyNumberProgression;
    private int currentEnemiesSpawned;



    public GameObject enemy;
    public List<GameObject> enemies;

    public float timeBetweenSpawns;
    private float lastTimeSpawned;

    public float spawnZoneWidth;
    public float spawnZoneHeight;

    private float currentSpawnZoneWidth;
    private float currentSpawnZoneHeight;


    public List<GameObject> zones;


    public GameObject selectionScreen;

    public List<Abilities> abilities;
    public List<string> selectedAbilities;

    private bool IsSelectionScreen;
   


    // Start is called before the first frame update
    void Start()
    {
        lastTimeSpawned = 0;
        wave = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Spawn(enemy);
        lastTimeSpawned = Time.time;
        currentEnemiesSpawned += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeBetweenSpawns + lastTimeSpawned && currentEnemiesSpawned < startEnemyNumber+(wave-1)*enemyNumberProgression && !selectionScreen.activeSelf)
        {
            Spawn(enemy);
            lastTimeSpawned = Time.time;
            currentEnemiesSpawned += 1;
        }
        else if (currentEnemiesSpawned >= startEnemyNumber + (wave - 1) * enemyNumberProgression && enemies.Count == 0 && !IsSelectionScreen)
        {
            StartCoroutine(WaitUntilSelectionScreen());
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
        enemies.Add(a);
        a.GetComponent<Enemy>().waveMan = gameObject.GetComponent<WaveManager>();
    }



    public void NextWave()
    {
        player.enabled = true;
        selectionScreen.SetActive(false);
        currentEnemiesSpawned = 0;
        wave += 1;
        lastTimeSpawned = 0;
        IsSelectionScreen = false;
    }


    void SelectionScreen()
    {
        player.enabled = false;
        selectionScreen.SetActive(true);

        foreach (Abilities a in abilities)
        {
            a.RemoveMyselfFromList();
        }

        foreach (Abilities a in abilities)
        {
            a.TakeRandomEnum();
        }
    }

    IEnumerator WaitUntilSelectionScreen()
    {
        IsSelectionScreen = true;
        yield return new WaitForSeconds(1f);
        SelectionScreen();
    }
}
