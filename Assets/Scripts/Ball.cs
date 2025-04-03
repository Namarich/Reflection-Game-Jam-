using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float MaxSpeed;
    private float speed;

    private bool wasPressed;

    public float speedReduction;

    private Vector2 direction;

    public bool isFirstBounce = true;

    public Color originalColor;
    public Color transparentColor;

    private float damage;

    private float lifeTime;
    private float startOfLifeTime;

    private bool isDoublingSpeedBullet;
    private int collisionCount;

    public bool isChainReaction;

    public GameObject explosion;
    public GameObject miniExplosion;

    public bool isExplosiveImpact;

    public float whoDiesFirst;

    public bool canHurtThePlayer;

    private WaveManager wav;

    // Start is called before the first frame update
    void Start()
    {
        //Reset();
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        if (wasPressed)
        {
            //transform.position += transform.up * -1 * Time.deltaTime * speed;
            transform.position += (Vector3)direction.normalized * speed * Time.deltaTime;
            //transform.right
        }

        if (speed <= MaxSpeed * 0.1 || Time.time >= startOfLifeTime+lifeTime)
        {
            Reset();
        }
            
    }

    private void Awake()
    {
        if (!canHurtThePlayer)
        {
            gameObject.GetComponent<SpriteRenderer>().color = transparentColor;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;
        }
        
        startOfLifeTime = Time.time;
        wav = GameObject.FindGameObjectWithTag("WaveManager").GetComponent<WaveManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (canHurtThePlayer)
        {
            isFirstBounce = false;
        }

        if (collision.gameObject.tag == "Wall")
        {
            isFirstBounce = false;
            speed *= speedReduction;
            collisionCount += 1;
            if (collisionCount % 2 == 0 && isDoublingSpeedBullet)
            {
                speed *= 2;
            }
        }
        else if (collision.gameObject.tag == "Enemy" && !isFirstBounce && !canHurtThePlayer)
        {
            if (!collision.gameObject.GetComponent<Enemy>().hasShield || collision.gameObject.GetComponent<Enemy>().shieldCollider != collision.collider)
            {
                speed *= collision.gameObject.GetComponent<Enemy>().bulletSpeedReduction;
                collision.gameObject.GetComponent<Enemy>().TakeDamage(damage,gameObject);
                //if (isChainReaction)
                //{
                    //GameObject a = wav.GetARandomEnemy(collision.gameObject);
                    //if (a != collision.gameObject)
                    //{
                        //a.GetComponent<Enemy>().TakeDamage(damage / 2, gameObject);
                    //}
                //}

                GameObject b = Instantiate(miniExplosion, transform.position, Quaternion.identity);
                b.GetComponent<ExplosionObject>().objectTag = "Enemy";
            }
            else
            {
                speed *= speedReduction;
            }
            //b.GetComponent<ExplosionObject>().explosionPower = 3f * speed;
        }
        else if (collision.gameObject.tag == "Bullet" && isExplosiveImpact)
        {
            whoDiesFirst = Random.Range(1, 100000001);
            

            if (whoDiesFirst >= collision.gameObject.GetComponent<Ball>().whoDiesFirst)
            {
                collision.gameObject.SetActive(false);
            }
            else
            {
                return;
            }
            GameObject a = Instantiate(explosion, transform.position, Quaternion.identity);
            a.GetComponent<ExplosionObject>().objectTag = "";
            a.GetComponent<ExplosionObject>().explosionDamage = damage * 2;
            gameObject.SetActive(false);
            GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().PlaySound("explosion");
        }
        else if (collision.gameObject.tag == "Player" && canHurtThePlayer)
        {
            speed *= speedReduction;
            collision.gameObject.GetComponent<Player>().TakeDamage(damage);
            GameObject b = Instantiate(miniExplosion, transform.position, Quaternion.identity);
            b.GetComponent<ExplosionObject>().objectTag = "Player";
        }

        Vector2 surfaceNormal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, surfaceNormal);
        if (!isFirstBounce && gameObject.GetComponent<SpriteRenderer>().color != originalColor)
        {
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;

        }
        Vector3 perpendicular = transform.position - (Vector3)direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        

    }




    public void Reset()
    {
        gameObject.SetActive(false);
    }

    public void ShootYourself(Vector2 _direction,Vector3 startPos,float _speed,float _lifeTime,float _damage, float _bulletSize,bool isDoublingSpeed,bool isChainReac,bool isExplosive)
    {
        GameObject b = Instantiate(miniExplosion, startPos, Quaternion.identity);
        b.GetComponent<ExplosionObject>().objectTag = "Player";
        transform.position = startPos;
        MaxSpeed = _speed;
        if (!canHurtThePlayer)
        {
            isFirstBounce = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;
        }
        direction = _direction;
        wasPressed = true;
        damage = _damage;
        gameObject.transform.localScale = new Vector3(0.4f,0.4f,0.4f) * _bulletSize;
        speed = MaxSpeed;
        Vector3 perpendicular = startPos - (Vector3)direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        lifeTime = _lifeTime;
        isDoublingSpeedBullet = isDoublingSpeed;
        isChainReaction = isChainReac;
        isExplosiveImpact = isExplosive;
        startOfLifeTime = Time.time;
        gameObject.GetComponent<SpriteRenderer>().color = transparentColor;
    }
}
