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
    public float projectileSpeedReduction;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DrawTrajectory();
        currentHealth = maxHealth;
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
        Vector3 perpendicular = transform.position - mousePos;

        Vector2 aimDirection = (Vector2)mousePos - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular*-1);
        rb.rotation = aimAngle;

    }


    public IEnumerator Shoot()
    {
        //Debug.DrawRay(shootPoint.position, shootPoint.up * 10, Color.green, 2f);


        
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, shootPoint.up);
        //Debug.DrawRay(shootPoint.position, (transform.position - shootPoint.position)*-5, Color.green,3f);

        GameObject a = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        a.GetComponent<Ball>().ShootYourself((shootPoint.position - transform.position ), shootPoint.position,projectileSpeed,projectileLifeTime,projectileDamage, projectileSpeedReduction,isDoublingSpeedBullet,isChainReaction,isExplosiveImpact);
        lastShotTime = Time.time;
        if (isExtraBullet)
        {
            yield return new WaitForSeconds(0.15f);
            ray = Physics2D.Raycast(shootPoint.position, shootPoint.up);
            a = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
            a.GetComponent<Ball>().ShootYourself((shootPoint.position - transform.position), shootPoint.position, projectileSpeed, projectileLifeTime, projectileDamage, projectileSpeedReduction,isDoublingSpeedBullet,isChainReaction,isExplosiveImpact);
        }
        yield return new WaitForSeconds(0);
    }

    public void DrawTrajectory()
    {
        Vector3 perpendicular = shootPoint.position - transform.position;
        //Instantiate(trajectory, shootPoint.position, Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, transform.up);
        
        float i = 1;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing)) < Vector2.Distance(shootPoint.position, ray.point))
        {
            GameObject a = Instantiate(trajectory, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing), Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
            trajectoryList.Add(a);
            a.transform.parent = trajectoryParent.transform;
            i += 1;
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

        float k = 1;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((k - 1) * trajectorySpacing)) > Vector2.Distance(shootPoint.position, ray.point))
        {
            GameObject b = trajectoryList[(int)k - 1];
            trajectoryList.Remove(b);
            Destroy(b);
            k+=2;
        }



        //Debug.DrawRay(shootPoint.position, ray.point, Color.green);
        float i = 1;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing)) < Vector2.Distance(shootPoint.position, ray.point))
        {
            if (i > trajectoryList.Count)
            {
                GameObject a = Instantiate(trajectory, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing), Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
                trajectoryList.Add(a);
                a.transform.parent = trajectoryParent.transform;
            }
            //GameObject a = Instantiate(trajectory, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing), Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
            trajectoryList[(int)i - 1].transform.position = shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing);
            trajectoryList[(int)i - 1].transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);
            i += 1;
        }

        while (i - 1 < trajectoryList.Count)
        {
            GameObject b = trajectoryList[(int)i - 1];
            trajectoryList.Remove(b);
            Destroy(b);
            i++;
        }

    }

    public void CreateDot(Vector3 perpendicular,int i)
    {
        GameObject a = Instantiate(trajectory, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing), Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
        trajectoryList.Add(a);
        a.transform.parent = trajectoryParent.transform;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(ChangeColor());
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    IEnumerator ChangeColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = damageColor;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
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
