using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position,playerTransform.position,speed);
        Vector3 dir = (playerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward,dir);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(ChangeColor());

        if (currentHealth <= 0)
        {
            waveMan.enemies.Remove(gameObject);
            Destroy(gameObject);
        }
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Attack(collision.gameObject.GetComponent<Player>());
        }

    }


    IEnumerator ChangeColor()
    {
        gameObject.GetComponent<SpriteRenderer>().color = damageColor;
        yield return new WaitForSeconds(0.15f);
        gameObject.GetComponent<SpriteRenderer>().color = originalColor;
    }


    public void Attack(Player player)
    {

        if (Time.time >= attackSpeed + lastTimeAttacked)
        {
            player.TakeDamage(damage);
            lastTimeAttacked = Time.time;
        }
        
    }
}
