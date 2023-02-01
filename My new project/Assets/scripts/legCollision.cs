using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class legCollision : MonoBehaviour
{
    public event Action LegCollisionEvent;
    public event Action<Transform> LegCollideEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("collider set" + other.name);
        
        if (other.name == "Tree_03")
        {
            LegCollisionEvent?.Invoke();
        }
            
    }
    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("collision set: " + collision.transform.name);
        LegCollideEvent?.Invoke(collision.transform);

        //if (collision.transform.name == "Fence_01")
        //{
            
        //}
    }
}
