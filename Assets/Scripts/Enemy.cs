using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    public WaveManager waveMan;
    private Transform playerTransform;

    public float speed;

    public float maxHealth;
    private float currentHealth;

    public Color originalColor;
    public Color damageColor;

    public float bulletSpeedReduction;

    public float damage;
    public float attackSpeed;
    public float lastTimeAttacked = 0;

    public GameObject damagePopup;
    public Color bigDamage;
    public Color smallDamage;

    public bool hasShield;
    public BoxCollider2D shieldCollider;

    public bool isSpawning;
    public GameObject spawnObject;
    public int spawnObjectNumber;

    public bool isRanged;
    public GameObject bullet;
    public float bulletSpeed;
    public Transform shootPoint;

    public RigidbodyConstraints2D noContraints;

    private bool canMove = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= attackSpeed + lastTimeAttacked && isRanged)
        {
            Shoot();
            lastTimeAttacked = Time.time;
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, speed);
        }
        
        //Vector3 dir = (playerTransform.position - transform.position).normalized;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward,dir);
        if (transform.position.x < playerTransform.position.x)
        {
            //gameObject.GetComponent<SpriteRenderer>().flipX = false;
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else  if (transform.position.x >= playerTransform.position.x)
        {
            //gameObject.GetComponent<SpriteRenderer>().flipX = true;
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }

    public void TakeDamage(float damage,GameObject whoHitMe)
    {
        currentHealth -= damage;
        DamagePopup(damage);
        StartCoroutine(ChangeColor(whoHitMe));

    }


    public void DamagePopup(float damage)
    {
        if (damage > 0)
        {
            GameObject a = Instantiate(damagePopup, transform.position, Quaternion.identity);
            a.GetComponent<TMP_Text>().color = Color.Lerp(smallDamage, bigDamage, damage / maxHealth);
            a.GetComponent<TMP_Text>().text = damage.ToString();
        }
        
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Attack(collision.gameObject.GetComponent<Player>());
        }

    }


    IEnumerator ChangeColor(GameObject whoHitMe)
    {
        if (currentHealth <= 0)
        {
            if (isSpawning)
            {
                StartCoroutine(Spawn());
            }
            else
            {
                StartCoroutine(DieGracefully(whoHitMe));

            }

        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = damageColor;
            yield return new WaitForSeconds(0.15f);
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;
        }
        
    }


    IEnumerator DieGracefully(GameObject whoHitMe)
    {
        canMove = false;
        gameObject.GetComponent<Rigidbody2D>().constraints = noContraints;
        gameObject.GetComponent<Rigidbody2D>().fixedAngle = false;
        foreach (BoxCollider2D a in gameObject.GetComponents<BoxCollider2D>())
        {
            a.enabled = false;
        }
        gameObject.GetComponent<Rigidbody2D>().AddExplosionForce(500,whoHitMe.transform.position,1);

        for (float i = 255; i > 0; i--)
        {
            gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, i);
            yield return new WaitForSeconds(0.25f/255.0f);
        }
        waveMan.enemies.Remove(gameObject);
        Destroy(gameObject);
    }


    public IEnumerator Spawn()
    {
        for (int i = 0; i < spawnObjectNumber;i++)
        {
            GameObject a = Instantiate(spawnObject, transform.position + new Vector3(Random.Range(-3f,3f), Random.Range(-3f, 3f), 0), Quaternion.identity);
            waveMan.enemies.Add(a);
            a.GetComponent<Enemy>().waveMan = waveMan;
            a.GetComponent<Enemy>().speed = Random.Range(a.GetComponent<Enemy>().speed*0.85f, a.GetComponent<Enemy>().speed*1.15f);
            yield return new WaitForSeconds(0.1f);
        }
        waveMan.enemies.Remove(gameObject);
        Destroy(gameObject);
    }


    public void Attack(Player player)
    {

        if (Time.time >= attackSpeed + lastTimeAttacked)
        {
            player.TakeDamage(damage);
            lastTimeAttacked = Time.time;
        }
        
    }

    public void Shoot()
    {

        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, ((shootPoint.position - playerTransform.position) * -1).normalized);
        Debug.DrawRay(shootPoint.position, (shootPoint.position - playerTransform.position) * -1, Color.green,3f);

        GameObject a = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        a.GetComponent<Ball>().miniExplosion = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().miniExplosion;
        a.GetComponent<Ball>().ShootYourself((shootPoint.position - playerTransform.position) * -1, shootPoint.position, bulletSpeed, 2f, damage, 0.6f, false, false,false);
    }
}
