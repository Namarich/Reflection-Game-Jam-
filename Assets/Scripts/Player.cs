using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (Input.GetMouseButtonDown(0) && Time.time >= lastShotTime + shotSpeed)
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

        Vector2 aimDirection = (Vector2)mousePos - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        //transform.rotation = Quaternion.LookRotation(Vector3.forward, perpendicular*-1);
        rb.rotation = aimAngle;

    }


    public void Shoot()
    {
        //Debug.DrawRay(shootPoint.position, shootPoint.up * 10, Color.green, 2f);



        RaycastHit2D ray = Physics2D.Raycast(shootPoint.position, shootPoint.up);


        Debug.DrawRay(shootPoint.position, (transform.position - shootPoint.position)*-5, Color.green,3f);

        GameObject a = Instantiate(bullet, shootPoint.position, shootPoint.rotation);
        a.GetComponent<Ball>().ShootYourself((transform.position - shootPoint.position), shootPoint);
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
}
