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
    public List<string> russianTutorials;
    public int tutorialStep = 1;
    public TMP_Text tutorialText;
    public GameObject tutorialPanel;
    public GameObject mirrorObject;


    public GameObject enemyInfoScreen;
    public TMP_Text bulletCountText;

    public ObjectPool circleSpawnController;

    public GameObject loseMenu;
    public TMP_Text loseWaveText;

    public bool isCursorVisibleAlways = true;


    // Start is called before the first frame update
    void Start()
    {

        lastTimeSpawned = 0;
        wave = 1;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "english")
        {
            waveText.text = $"Wave {wave}";
        }
        else if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "russian")
        {
            waveText.text = $"����� {wave}";
        }
        tutorialPanel.SetActive(true);
        if (!isCursorVisibleAlways)
        {
            Cursor.visible = false;
        }

        if (PlayerPrefs.GetString("language") == "english")
        {
            waveText.text = $"Wave {wave}";
        }
        else if (PlayerPrefs.GetString("language") == "russian")
        {
            waveText.text = $"����� {wave}";
        }
        player.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= timeBetweenSpawns + lastTimeSpawned - enemyCircleDuration && currentEnemiesSpawned < startEnemyNumber + (wave - 1) * enemyNumberProgression && !selectionScreen.activeSelf && !startedSpawning && !isTutorial)
        {
            //Spawn(enemy);
            CanSummonEnemies();
            //int howMany = Random.Range(1, (wave / 3) + 2);
            //while (howMany+currentEnemiesSpawned > startEnemyNumber + (wave - 1) * enemyNumberProgression)
            //{
            //howMany = Random.Range(1, (wave / 3) + 2);
            //}

            int howMany = Random.Range(1, ((wave / 3) + 2) % (startEnemyNumber + (wave - 1) * enemyNumberProgression));
            for (int i = 0; i < howMany; i++)
            {
                Spawn(canSummonEnemies[Random.Range(0, canSummonEnemies.Count)]);
            }
        }
        else if (currentEnemiesSpawned >= startEnemyNumber + (wave - 1) * enemyNumberProgression && CountAllCurrentlyAliveEnemies() == 0 && !IsSelectionScreen && !isTutorial)
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

        bulletCountText.text = $"{GetComponent<ObjectPool>().HowManyInactive()}x";


        if (PlayerPrefs.GetString("language") == "english")
        {
            waveText.text = $"Wave {wave}";
        }
        else if (PlayerPrefs.GetString("language") == "russian")
        {
            waveText.text = $"����� {wave}";
        }
    }


    public void Spawn(GameObject enemy)
    {
        startedSpawning = true;

        Bounds currentBounds = zones[Random.Range(0, zones.Count)].GetComponent<SpriteRenderer>().bounds;

        Vector2 spawnPos = RandomPointInsideBounds(currentBounds, enemy.GetComponent<SpriteRenderer>().bounds.size);

        StartCoroutine(SpawnEnemy(spawnPos, enemy));

    }


    public Vector2 RandomPointInsideBounds(Bounds bounds, Vector3 size)
    {
        return new Vector2(Random.Range(bounds.min.x + size.x, bounds.max.x - size.x), Random.Range(bounds.min.y + size.y, bounds.max.y - size.y));
    }



    IEnumerator SpawnEnemy(Vector3 spawnPos, GameObject enemy)
    {
        //GameObject b = Instantiate(enemySpawnCircle, spawnPos, Quaternion.identity);
        GameObject b = circleSpawnController.GetPooledObject();
        if (b != null)
        {
            b.SetActive(true);
            b.transform.position = spawnPos;
        }
        GameObject a = Instantiate(enemy, spawnPos, Quaternion.identity);
        enemies.Add(a);
        a.GetComponent<Enemy>().waveMan = gameObject.GetComponent<WaveManager>();
        a.SetActive(false);
        a.GetComponent<Enemy>().enabled = false;
        yield return new WaitForSeconds(enemyCircleDuration);
        //Destroy(b);
        if (b != null)
        {
            b.SetActive(false);
        }
        if (a != null)
        {
            a.SetActive(true);
            a.GetComponent<Enemy>().enabled = true;
            currentEnemiesSpawned += 1;
            lastTimeSpawned = Time.time;
        }

        startedSpawning = false;
    }



    public void NextWave()
    {
        DeleteAllEnemies();
        int n = CountAllCurrentlyAliveEnemies();
        player.gameObject.SetActive(true);
        player.playerDamageAnim.enabled = true;
        if (isTutorial)
        {
            tutorialPanel.SetActive(true);
        }
        player.enabled = true;
        player.currentHealth = player.maxHealth;
        selectionScreen.SetActive(false);
        fightingScreen.SetActive(true);
        currentEnemiesSpawned = 0;
        if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "english")
        {
            waveText.text = $"Wave {wave}";
        }
        else if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "russian")
        {
            waveText.text = $"����� {wave}";
        }

        lastTimeSpawned = 0;
        IsSelectionScreen = false;
        Resources.UnloadUnusedAssets();
        if (!isCursorVisibleAlways)
        {
            Cursor.visible = false;
        }
        currentEnemiesSpawned = 0;
        startedSpawning = false;
        

    }

    public void DeleteAllEnemies()
    {
        foreach (GameObject a in enemies)
        {
            Destroy(a);
        }
        enemies.RemoveAll(GameObject => !GameObject);
    }

    public void DeleteAllProjectiles()
    {
        ObjectPool pool = gameObject.GetComponent<ObjectPool>();
        foreach (GameObject a in pool.pooledObjects)
        {
            a.SetActive(false);
        }
    }


    public void EnemyInfoScreen()
    {
        //tutorialPanel.SetActive(false);
        if (isTutorial)
        {
            tutorialPanel.SetActive(true);
        }
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
        Cursor.visible = true;
        player.playerDamageAnim.SetBool("wasPlayerDamaged", false);
        player.playerDamageAnim.enabled = false;
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
        yield return new WaitForSeconds(0.2f);
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

    public int CountAllCurrentlyAliveEnemies()
    {
        int count = 0;

        foreach (GameObject a in enemies)
        {
            if (a != null)
            {
                count++;
            }
        }
        enemies.RemoveAll(GameObject => !GameObject);
        return count;

        
    }


    void Tutorial()
    {
        if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "english")
        {
            tutorialText.text = tutorials[tutorialStep - 1];
        }
        else if (GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language == "russian")
        {
            tutorialText.text = russianTutorials[tutorialStep - 1];
        }


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
            if (CountAllCurrentlyAliveEnemies() == 0 && currentEnemiesSpawned == 0)
            {
                Spawn(enemyList[0].enemy);
                currentEnemiesSpawned += 1;
            }
            else if (CountAllCurrentlyAliveEnemies() == 0 && currentEnemiesSpawned > 0)
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

            }
        }

        if (tutorialStep == 5)
        {
            if (Input.GetMouseButtonDown(0))
            {
                tutorialStep += 1;
                isTutorial = false;
            }
        }

        if (tutorialStep == 6)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                tutorialStep += 1;
            }
        }

        if (tutorialStep == 7)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                tutorialPanel.SetActive(false);
            }
        }
    }


    public GameObject GetARandomEnemy(GameObject exceptThisOne)
    {
        GameObject a = enemies[0];
        if (CountAllCurrentlyAliveEnemies() > 1)
        {
            while (a == exceptThisOne || !a.activeInHierarchy || !a.GetComponent<Enemy>().canMove)
            {
                a = enemies[Random.Range(0, enemies.Count)];
            }

        }
        return a;
    }


    public void Lose()
    {
        loseMenu.SetActive(true);
        selectionScreen.SetActive(true);
        IsSelectionScreen = true;
        SelectionScreen();
        tutorialPanel.SetActive(false);
        fightingScreen.SetActive(false);
        if (PlayerPrefs.GetString("language") == "english")
        {
            loseWaveText.text = $"Wave {wave}";
        }
        else if (PlayerPrefs.GetString("language") == "russian")
        {
            loseWaveText.text = $"����� {wave}";
        }
        Debug.Log($"{PlayerPrefs.GetString("language")};{GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>().language}");
        //player.gameObject.SetActive(false);
        CanSummonEnemies();
        foreach (GameObject a in enemies)
        {
            Destroy(a);
        }

        currentEnemiesSpawned = 0;
        
    }

}
