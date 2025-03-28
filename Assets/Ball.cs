using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public float MaxSpeed;
    private float speed;

    private bool wasPressed;
    private bool wasCollision;

    public float speedReduction;

    public Camera cam;

    private float ZRot;

    private Transform startTransform;

    private Vector2 direction;

    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!wasPressed)
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 perpendicular = transform.position - mousePos;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        }

        if (Input.GetMouseButtonDown(0) && !wasPressed)
        {
            //rb.AddForce(new Vector2(10,10));
            wasPressed = true;
            startTransform = gameObject.transform;
            direction = mousePos - transform.position;
        }

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
        speed = MaxSpeed;
        transform.position = new Vector3(0, 0, 0);
        transform.eulerAngles = new Vector3(0, 0, 0);
        wasPressed = false;
    }
}
