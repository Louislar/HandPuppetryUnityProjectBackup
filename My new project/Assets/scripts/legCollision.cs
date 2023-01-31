using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class legCollision : MonoBehaviour
{
    public event Action LegCollisionEvent;

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
}
