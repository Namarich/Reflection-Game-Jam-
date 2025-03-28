using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    public float speed;

    private bool wasPressed;
    private bool wasCollision;

    public float speedReduction;

    public Camera cam;

    private float ZRot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!wasPressed)
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 perpendicular = transform.position - mousePos;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        }
        



        if (Input.GetMouseButtonDown(0) && !wasPressed)
        {
            //rb.AddForce(new Vector2(10,10));
            wasPressed = true;
            
        }

        if (wasPressed)
        {
            transform.position += transform.up * -1 * Time.deltaTime * speed;
            //transform.right
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log($"{transform.eulerAngles.z};{transform.eulerAngles.z-360}");
        //transform.rotation = Quaternion.Euler(0,0,transform.rotation.z-360);
        transform.eulerAngles = new Vector3(0, 0, 180-transform.eulerAngles.z);
        speed = speed * speedReduction;
    }
}
