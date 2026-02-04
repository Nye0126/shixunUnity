using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newbullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10f;
    public float lifeTime = 2f;
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}