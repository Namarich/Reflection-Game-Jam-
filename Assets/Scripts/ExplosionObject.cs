using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObject : MonoBehaviour
{

    public float explosionPower;
    public float explosionDamage;
    public float explosionRadius;

    public float lifeTime;
    private float startOfLifeTime;

    public float disableTime;
    private bool isInactive = false;

    public Color invisColor;

    public RigidbodyConstraints2D enemyDefaultContraints;
    public RigidbodyConstraints2D enemyExplosionContraints;

    // Start is called before the first frame update

    private void Awake()
    {
        startOfLifeTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime+startOfLifeTime <= Time.time)
        {
            isInactive = true;
            gameObject.GetComponent<SpriteRenderer>().color = invisColor;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            if (lifeTime + startOfLifeTime <= Time.time - disableTime * 2)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Wall" && !isInactive)
        {
            if (collision.tag == "Player")
            {
                collision.gameObject.GetComponent<Player>().TakeDamage(explosionDamage);
                StartCoroutine(DisablePlayer(collision));
            }

            if (collision.tag == "Enemy")
            {
                collision.gameObject.GetComponent<Rigidbody2D>().freezeRotation = false;
                collision.gameObject.GetComponent<Rigidbody2D>().constraints = enemyExplosionContraints;
                collision.gameObject.GetComponent<Enemy>().TakeDamage(explosionDamage);
                StartCoroutine(DisableEnemy(collision));
                
            }

            collision.GetComponent<Rigidbody2D>().AddExplosionForce(explosionPower,transform.position,explosionRadius);
        }
        Debug.Log(collision.GetComponent<Rigidbody2D>().velocity);
    }

    IEnumerator DisablePlayer(Collider2D collision)
    {
        collision.gameObject.GetComponent<Player>().canMove = false;
        yield return new WaitForSeconds(disableTime);
        if (collision.gameObject)
        {
            collision.gameObject.GetComponent<Player>().canMove = true;
        }
        
    }

    IEnumerator DisableEnemy(Collider2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().enabled = false;
        yield return new WaitForSeconds(disableTime);
        if (collision.gameObject)
        {
            collision.gameObject.GetComponent<Enemy>().enabled = true;
            collision.gameObject.GetComponent<Rigidbody2D>().freezeRotation = true;
            collision.gameObject.GetComponent<Rigidbody2D>().constraints = enemyDefaultContraints;
        }
        
    }
}
