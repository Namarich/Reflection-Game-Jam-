using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{

    private Player player;

    public int wave;
    public int startEnemyNumber;
    public int enemyNumberProgression;
    private int currentEnemiesSpawned;


    [System.Serializable]
    public class Ghost
    {
        public GameObject enemy;
        public int startingWave;
    }

    public List<Ghost> enemyList;

    public List<GameObject> canSummonEnemies;


    public List<GameObject> enemies;

    public float timeBetweenSpawns;
    private float lastTimeSpawned;

    public float spawnZoneWidth;
    public float spawnZoneHeight;

    private float currentSpawnZoneWidth;
    private float currentSpawnZoneHeight;


    public List<GameObject> zones;


    public GameObject selectionScreen;
    public GameObject fightingScreen;

    public List<Abilities> abilities;
    public List<string> selectedAbilities;

    public List<string> cannotUseAbilities;

    public bool IsSelectionScreen;

    public GameObject enemySpawnCircle;
    public float enemyCircleDuration;

    private bool startedSpawning;


    public TMP_Text waveText;


   


    // Start is called before the first frame update
    void Start()
    {
        lastTimeSpawned = 0;
        wave = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        waveText.text = $"Wave {wave}";
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeBetweenSpawns + lastTimeSpawned - enemyCircleDuration && currentEnemiesSpawned < startEnemyNumber+(wave-1)*enemyNumberProgression && !selectionScreen.activeSelf && !startedSpawning)
        {
            //Spawn(enemy);
            CanSummonEnemies();
            int howMany = Random.Range(1, (wave / 3) + 2);
            while (howMany+currentEnemiesSpawned > startEnemyNumber + (wave - 1) * enemyNumberProgression)
            {
                howMany = Random.Range(1, (wave / 3) + 2);
            }
            for (int i = 0;i < howMany; i++)
            {
                Spawn(canSummonEnemies[Random.Range(0, canSummonEnemies.Count)]);
            }  
        }
        else if (currentEnemiesSpawned >= startEnemyNumber + (wave - 1) * enemyNumberProgression && enemies.Count == 0 && !IsSelectionScreen)
        {
            StartCoroutine(WaitUntilSelectionScreen());
        }
        //Debug.Log($"wave:{wave}");
    }


    public void Spawn(GameObject enemy)
    {
        startedSpawning = true;
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
        StartCoroutine(SpawnEnemy(spawnPos,enemy));
        
    }



    IEnumerator SpawnEnemy(Vector3 spawnPos,GameObject enemy)
    {
        GameObject b = Instantiate(enemySpawnCircle, spawnPos, Quaternion.identity);
        yield return new WaitForSeconds(enemyCircleDuration);
        Destroy(b);
        GameObject a = Instantiate(enemy, spawnPos, Quaternion.identity);
        enemies.Add(a);
        a.GetComponent<Enemy>().waveMan = gameObject.GetComponent<WaveManager>();
        currentEnemiesSpawned += 1;
        lastTimeSpawned = Time.time;
        startedSpawning = false;
    }



    public void NextWave()
    {
        
        player.enabled = true;
        player.currentHealth = player.maxHealth;
        selectionScreen.SetActive(false);
        fightingScreen.SetActive(true);
        currentEnemiesSpawned = 0;
        wave += 1;
        waveText.text = $"Wave {wave}";
        lastTimeSpawned = 0;
        IsSelectionScreen = false;
    }


    void SelectionScreen()
    {
        player.gameObject.transform.position = transform.position;
        player.enabled = false;
        selectionScreen.SetActive(true);
        fightingScreen.SetActive(false);

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




    public void CanSummonEnemies()
    {

        foreach (GameObject a in canSummonEnemies)
        {
            canSummonEnemies.Remove(a);
        }

        foreach (Ghost a in enemyList)
        {
            if (a.startingWave <= wave)
            {
                canSummonEnemies.Add(a.enemy);
            }
        }
    }
}
