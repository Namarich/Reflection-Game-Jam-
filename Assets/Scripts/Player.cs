using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float smoothTime = 0.1f; // Adjust for smooth acceleration
    private Vector2 input;
    private Vector2 velocity = Vector2.zero;
    private Rigidbody2D rb;

    public Camera cam;

    private Vector3 mousePos;

    [SerializeField] private GameObject bullet;

    [SerializeField] private Transform shootPoint;

    [SerializeField] private float shotSpeed;
    private float lastShotTime;

    public GameObject trajectory;
    public List<GameObject> trajectoryList;
    public float trajectorySpacing;
    [SerializeField] private GameObject trajectoryParent;
    public int numOfDots;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DrawTrajectory();
    }

    void Update()
    {
        // Get Input (WASD / Arrow Keys)
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize(); // Prevent faster diagonal movement

        if (Input.GetMouseButtonDown(0) && Time.time >= lastShotTime+shotSpeed)
        {
            Shoot();
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

        
        
    }

    void FixedUpdate()
    {
        // Smooth movement using SmoothDamp
        Vector2 targetVelocity = input * moveSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocity, smoothTime);

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 perpendicular = transform.position - mousePos;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular*-1);
    }


    public void Shoot()
    {
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, transform.up); 

        GameObject a = Instantiate(bullet, shootPoint.position, Quaternion.identity);
        a.GetComponent<Ball>().ShootYourself(ray.point, shootPoint);
        lastShotTime = Time.time;
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

    public void UpdateTrajectory()
    {
        Vector3 perpendicular = shootPoint.position - transform.position;
        //Instantiate(trajectory, shootPoint.position, Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, transform.up);


        float i = 1;
        while (Vector2.Distance(shootPoint.position, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing)) < Vector2.Distance(shootPoint.position, ray.point))
        {
            //GameObject a = Instantiate(trajectory, shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing), Quaternion.LookRotation(Vector3.forward, perpendicular * -1));
            trajectoryList[(int)i - 1].transform.position = shootPoint.position + gameObject.transform.up * ((i - 1) * trajectorySpacing);
            trajectoryList[(int)i - 1].transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular * -1);
            i += 1;
        }

        Vector2 dir = Vector2.Reflect(shootPoint.position, ray.normal);
        for (float k = i; k < trajectoryList.Count+1; k++)
        {
            //Vector3 extended_vector = shootPoint.position * i * trajectorySpacing;
            trajectoryList[(int)k - 1].transform.position = ray.point + dir * ((k - i) * trajectorySpacing);
            perpendicular = trajectoryList[(int)k - 1].transform.position - (Vector3)dir;
            trajectoryList[(int)k - 1].transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular);
        }
    }
}
