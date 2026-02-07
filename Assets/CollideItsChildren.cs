using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideItsChildren : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Collider2D>() == null)
            {
                child.gameObject.AddComponent<BoxCollider2D>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
