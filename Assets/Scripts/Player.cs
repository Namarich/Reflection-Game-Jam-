using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float smoothTime = 0.1f; // Adjust for smooth acceleration
    private Vector2 input;
    private Vector2 velocity = Vector2.zero;
    public Rigidbody2D rb;

    public Camera cam;

    private Vector3 mousePos;

    [SerializeField] private GameObject bullet;

    [SerializeField] private Transform shootPoint;

    public float shotSpeed;
    private float lastShotTime;

    public GameObject trajectory;
    public List<GameObject> trajectoryList;
    public float trajectorySpacing;
    [SerializeField] private GameObject trajectoryParent;
    public int numOfDots;


    public float maxHealth;
    public float currentHealth;
    public Color originalColor;
    public Color damageColor;

    public float projectileSpeed;
    public float projectileDamage;
    public float projectileSize;
    public float projectileLifeTime;

    public bool isExtraBullet;

    public bool isDoublingSpeedBullet;

    public bool isChainReaction;

    public bool canMove = true;

    public bool isExplosiveImpact;


    public Image ammoUI;
    public Image healthBar;
    float lerpSpeed;

    public Color maxColor;
    public Color minColor;

    public Animator playerDamageAnim;
    public Animator camAnim;

    public GameObject miniExplosion;

    private WaveManager wav;

    public bool hasRevived = false;
    public GameObject reviveAdGameObject;

    public ButtonManager buttonManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        
        currentHealth = maxHealth;
        wav = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>();
        //DrawTrajectory();
    }

    void Update()
    {
        // Get Input (WASD / Arrow Keys)
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize(); // Prevent faster diagonal movement

        if (Input.GetMouseButtonDown(0) && Time.time >= lastShotTime + shotSpeed)
        {
            StartCoroutine(Shoot());
        }

        if (Time.time >= lastShotTime + shotSpeed)
        {
            trajectoryParent.SetActive(true);
            UpdateTrajectory();
        }
        else
        {
            trajectoryParent.SetActive(false);
        }

        ammoUI.fillAmount = (Time.time - lastShotTime) / shotSpeed;
        FillTheHealthBar();

        if (hasRevived)
        {
            reviveAdGameObject.SetActive(false);
        }
    }


    public void FillTheHealthBar()
    {
        lerpSpeed = 3f * Time.deltaTime;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount,currentHealth/maxHealth,lerpSpeed);

        Color healthColor = Color.Lerp(maxColor, minColor, currentHealth / maxHealth);
        healthBar.color = healthColor;
    }

    void FixedUpdate()
    {
        // Smooth movement using SmoothDamp
        Vector2 targetVelocity = input * moveSpeed;
        if (canMove)
        {
            rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothTime);
        }
        

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 perpendicular = transform.position - mousePos;

        Vector2 aimDirection = mousePos - transform.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular*-1);
        rb.rotation = aimAngle;

    }


    public IEnumerator Shoot()
    {
        //Debug.DrawRay(shootPoint.position, shootPoint.up * 10, Color.green, 2f);
        
        //Debug.DrawRay(shootPoint.position, (transform.position - shootPoint.position)*-5, Color.green,3f);

        GameObject a = wav.gameObject.GetComponent<ObjectPool>().GetPooledObject();
        if (a != null)
        {
            a.SetActive(true);
            a.GetComponent<Ball>().miniExplosion = miniExplosion;
            a.GetComponent<Ball>().ShootYourself((shootPoint.position - transform.position), shootPoint.position, projectileSpeed, projectileLifeTime, projectileDamage, projectileSize, isDoublingSpeedBullet, isChainReaction, isExplosiveImpact);
            lastShotTime = Time.time;
            wav.projectiles.Add(a);
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("shoot3");
        }
        
        if (isExtraBullet)
        {
            yield return new WaitForSeconds(0.15f);
            //ray = Physics2D.Raycast(shootPoint.position, shootPoint.up);
            a = wav.gameObject.GetComponent<ObjectPool>().GetPooledObject();
            if (a != null)
            {
                a.SetActive(true);
                a.GetComponent<Ball>().miniExplosion = miniExplosion;
                a.GetComponent<Ball>().ShootYourself((shootPoint.position - transform.position), shootPoint.position, projectileSpeed, projectileLifeTime, projectileDamage, projectileSize, isDoublingSpeedBullet, isChainReaction, isExplosiveImpact);
                wav.projectiles.Add(a);
                GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("shoot3");
            }
            
        }
        //yield return new WaitForSeconds(0);
    }

    public void DrawTrajectory()
    {
        while (gameObject.GetComponent<ObjectPool>().amountToPool != gameObject.GetComponent<ObjectPool>().pooledObjects.Count)
        {
            gameObject.GetComponent<ObjectPool>().StartPool();
        }
        Vector3 perpendicular = shootPoint.position - transform.position;
        //Instantiate(trajectory, shootPoint.position, Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, transform.up);
        
        float i = 1;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing)) < Vector2.Distance(shootPoint.position, ray.point))
        {
            GameObject a = gameObject.GetComponent<ObjectPool>().GetPooledObject();
            if (a != null)
            {
                a.SetActive(true);
                a.transform.position = shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing);
                a.transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);
                a.transform.parent = trajectoryParent.transform;
            }
            else
            {
                break;
            }
            i++;
            
        }

    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<Enemy>().Attack(gameObject.GetComponent<Player>());
        }
    }




    public void UpdateTrajectory()
    {
        Vector3 perpendicular = shootPoint.position - transform.position;
        //Instantiate(trajectory, shootPoint.position, Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, transform.up);

        float k = 30;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((k - 1) * trajectorySpacing)) > Vector2.Distance(shootPoint.position, ray.point))
        {
            GetComponent<ObjectPool>().pooledObjects[(int)k - 1].SetActive(false);
            k--;
            if (k == 0)
            {
                break;
            }
            
        }



        //Debug.DrawRay(shootPoint.position, ray.point, Color.green);
        float i = 1;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing)) < Vector2.Distance(shootPoint.position, ray.point))
        {
            if (i > 30-gameObject.GetComponent<ObjectPool>().HowManyInactive())
            {
                GameObject a = gameObject.GetComponent<ObjectPool>().GetPooledObject();
                if (a != null)
                {
                    a.SetActive(true);
                    a.transform.position = shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing);
                    a.transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);
                    a.transform.parent = trajectoryParent.transform;
                }
                
            }
            //GameObject a = Instantiate(trajectory, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing), Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
            gameObject.GetComponent<ObjectPool>().pooledObjects[(int)i - 1].transform.position = shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing);
            gameObject.GetComponent<ObjectPool>().pooledObjects[(int)i - 1].transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);
            
            i += 1;
        }

    }



    public void TakeDamage(float damage)
    {
        if (damage > 0)
        {
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("hit4");
            currentHealth -= damage;
            StartCoroutine(ChangeColor());
            if (currentHealth <= 0)
            {
                playerDamageAnim.SetBool("wasPlayerDamaged", false);
                buttonManager.startOfReviveAd = Time.time;
                reviveAdGameObject.SetActive(true);
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                wav.DeleteAllEnemies();
                wav.DeleteAllProjectiles();
                currentHealth = 1;
                //Invoke("Reset", durationOfReviveAd);
            }
        }
        
    }

    public void Reset()
    {
        if (!hasRevived)
        {
            wav.Lose();
        }
        else
        {
            currentHealth = maxHealth;
            wav.NextWave();
        }
        hasRevived = false;
        reviveAdGameObject.SetActive(false);
        lastShotTime = 0f;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
        FillTheHealthBar();
        wav.tutorialStep = 1;
    }

    IEnumerator ChangeColor()
    {
        playerDamageAnim.SetBool("wasPlayerDamaged", true);
        yield return new WaitForSeconds(0.01f);
        playerDamageAnim.SetBool("wasPlayerDamaged", false);
    }

    public void Heal(float hp)
    {
        currentHealth += hp;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
