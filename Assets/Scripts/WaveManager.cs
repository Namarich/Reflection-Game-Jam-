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
        public GameObject UIPanel;
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

    public List<GameObject> projectiles;

    public bool isTutorial = true;
    public List<string> tutorials;
    private int tutorialStep = 1;
    public TMP_Text tutorialText;
    public GameObject tutorialPanel;
    public GameObject mirrorObject;


    public GameObject enemyInfoScreen;


    // Start is called before the first frame update
    void Start()
    {
        lastTimeSpawned = 0;
        wave = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        waveText.text = $"Wave {wave}";
        tutorialPanel.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeBetweenSpawns + lastTimeSpawned - enemyCircleDuration && currentEnemiesSpawned < startEnemyNumber+(wave-1)*enemyNumberProgression && !selectionScreen.activeSelf && !startedSpawning && !isTutorial)
        {
            //Spawn(enemy);
            CanSummonEnemies();
            //int howMany = Random.Range(1, (wave / 3) + 2);
            //while (howMany+currentEnemiesSpawned > startEnemyNumber + (wave - 1) * enemyNumberProgression)
            //{
                //howMany = Random.Range(1, (wave / 3) + 2);
            //}

            int howMany = Random.Range(1, ((wave / 3) + 2) % (startEnemyNumber + (wave - 1) * enemyNumberProgression));
            for (int i = 0;i < howMany; i++)
            {
                Spawn(canSummonEnemies[Random.Range(0, canSummonEnemies.Count)]);
            }  
        }
        else if (currentEnemiesSpawned >= startEnemyNumber + (wave - 1) * enemyNumberProgression && enemies.Count == 0 && !IsSelectionScreen && !isTutorial)
        {
            StartCoroutine(WaitUntilSelectionScreen());
        }
        //Debug.Log($"wave:{wave}");
        if (tutorialPanel.activeSelf)
        {
            Tutorial();
        }

        if (enemyInfoScreen.activeSelf && Input.GetMouseButtonDown(0))
        {
            NextWave();
            enemyInfoScreen.SetActive(false);
        }
        
    }


    public void Spawn(GameObject enemy)
    {
        startedSpawning = true;

        Bounds currentBounds = zones[Random.Range(0,zones.Count)].GetComponent<SpriteRenderer>().bounds;

        Vector2 spawnPos = RandomPointInsideBounds(currentBounds,enemy.GetComponent<SpriteRenderer>().bounds.size);

        StartCoroutine(SpawnEnemy(spawnPos,enemy));
        
    }


    public Vector2 RandomPointInsideBounds(Bounds bounds,Vector3 size)
    {
        return new Vector2(Random.Range(bounds.min.x + size.x, bounds.max.x - size.x), Random.Range(bounds.min.y + size.y, bounds.max.y - size.y));
    }



    IEnumerator SpawnEnemy(Vector3 spawnPos,GameObject enemy)
    {
        GameObject b = Instantiate(enemySpawnCircle, spawnPos, Quaternion.identity);
        GameObject a = Instantiate(enemy, spawnPos, Quaternion.identity);
        enemies.Add(a);
        a.GetComponent<Enemy>().waveMan = gameObject.GetComponent<WaveManager>();
        a.SetActive(false);
        a.GetComponent<Enemy>().enabled = false;
        yield return new WaitForSeconds(enemyCircleDuration);
        Destroy(b);
        a.SetActive(true);
        a.GetComponent<Enemy>().enabled = true;
        currentEnemiesSpawned += 1;
        lastTimeSpawned = Time.time;
        startedSpawning = false;
    }



    public void NextWave()
    {
        if (isTutorial)
        {
            tutorialPanel.SetActive(true);
        }
        player.enabled = true;
        player.currentHealth = player.maxHealth;
        selectionScreen.SetActive(false);
        fightingScreen.SetActive(true);
        currentEnemiesSpawned = 0;
        waveText.text = $"Wave {wave}";
        lastTimeSpawned = 0;
        IsSelectionScreen = false;
    }


    public void EnemyInfoScreen()
    {
        tutorialPanel.SetActive(false);
        enemyInfoScreen.SetActive(true);
        wave += 1;
        foreach (Ghost a in enemyList)
        {
            a.UIPanel.SetActive(false);
            if (a.startingWave <= wave)
            {
                a.UIPanel.SetActive(true);
            }
        }
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

        canSummonEnemies = new List<GameObject>();

        foreach (Ghost a in enemyList)
        {
            if (a.startingWave <= wave)
            {
                canSummonEnemies.Add(a.enemy);
            }
        }
    }


    void Tutorial()
    {
        tutorialText.text = tutorials[tutorialStep - 1];

        if (tutorialStep == 1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                tutorialStep += 1;
            }
        }


        if (tutorialStep == 2)
        {
            mirrorObject.SetActive(true);
            if (projectiles.Count >= 1)
            {
                tutorialStep += 1;
                mirrorObject.SetActive(false);
            }
        }

        if (tutorialStep == 3)
        {
            mirrorObject.SetActive(false);
            if (enemies.Count == 0 && currentEnemiesSpawned == 0)
            {
                Spawn(enemyList[0].enemy);
                currentEnemiesSpawned += 1;
            }
            else if (enemies.Count == 0 && currentEnemiesSpawned > 0)
            {
                tutorialStep += 1;
                StartCoroutine(WaitUntilSelectionScreen());
            }
            
        }

        if (tutorialStep == 4)
        {
            if (wave > 1)
            {
                tutorialStep += 1;
                isTutorial = false;
            }
        }

        if (tutorialStep == 5)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                tutorialPanel.SetActive(false);
            }
        }
    }
}
