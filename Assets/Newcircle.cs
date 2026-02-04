using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Newcircle : MonoBehaviour //define Íæ¼Ò²Ù¿ØÎïÌå
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 700f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (rb != null)//wasd input
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            moveInput = new Vector2(moveX, moveY).normalized;
            SmoothRotate();
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
          

        }
    }
    void Shoot()
    {
        Instantiate(bullet,transform.position,transform.rotation);//create bullet
    }
    void Moveplayer()
    {
       
            rb.velocity = moveInput * moveSpeed;
       
    }
    
    void FixedUpdate()
    {
        Moveplayer();
        SmoothRotate();//add smooth ratation
    }
    void SmoothRotate()
    {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        Vector2 lookDir = (Vector2)mousePosition - rb.position;


        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;

        float smoothedAngle = Mathf.MoveTowardsAngle(rb.rotation, angle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothedAngle);
    }



}
