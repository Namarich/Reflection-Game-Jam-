using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private float MaxSpeed;
    private float speed;

    private bool wasPressed;

    private float speedReduction;

    private Vector2 direction;

    public bool isFirstBounce = true;

    public Color originalColor;
    public Color transparentColor;

    private float damage;

    private float lifeTime;
    private float startOfLifeTime;

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
        gameObject.GetComponent<SpriteRenderer>().color = transparentColor;
        startOfLifeTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Wall")
        {
            isFirstBounce = false;
            speed *= speedReduction;
        }
        else if (collision.gameObject.tag == "Enemy" && !isFirstBounce)
        {
            speed *= collision.gameObject.GetComponent<Enemy>().bulletSpeedReduction;
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        Vector2 surfaceNormal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, surfaceNormal);
        if (!isFirstBounce)
        {
            gameObject.GetComponent<SpriteRenderer>().color = originalColor;

        }
        Vector3 perpendicular = transform.position - (Vector3)direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);

    }




    public void Reset()
    {
        Destroy(gameObject);
    }

    public void ShootYourself(Vector2 _direction,Transform startPos,float _speed,float _lifeTime,float _damage, float _speedReduction)
    {
        MaxSpeed = _speed;
        isFirstBounce = true;
        direction = _direction;
        wasPressed = true;
        damage = _damage;
        speedReduction = _speedReduction;
        speed = MaxSpeed;
        Vector3 perpendicular = startPos.position - (Vector3)direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);
        lifeTime = _lifeTime;
        

    }
}
