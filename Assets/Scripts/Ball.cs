using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public float MaxSpeed;
    private float speed;

    private bool wasPressed;

    public float speedReduction;

    private Vector2 direction;

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

        if (speed <= MaxSpeed * 0.1)
        {
            Reset();
        }
            
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 surfaceNormal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, surfaceNormal);
        speed *= speedReduction;
        Vector3 perpendicular = transform.position - (Vector3)direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
    }

    public void Reset()
    {
        Destroy(gameObject);
    }

    public void ShootYourself(Vector2 _direction,Transform startPos)
    {
        direction = _direction;
        wasPressed = true;
        speed = MaxSpeed;
        Vector3 perpendicular = startPos.position - (Vector3)direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);

    }
}
