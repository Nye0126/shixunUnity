using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newbullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10f;
    public float lifeTime = 2f;
    public float hurt = 50f;

    public string owner;
    
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
      
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(hurt); // ¿ÛÑª
            Destroy(gameObject);  // Ïú»Ù×Óµ¯

        }
    }
}