using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed = 10f;
    public float lifeTime = 2f;

    public string owner;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed;
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
            if (owner != "enemy")
            {
                enemy.TakeDamage(100); // ¿Û 20 µãÑª
                Destroy(gameObject);  // Ïú»Ù×Óµ¯
            }

        }
    }
}