using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MonoBehaviour
{
    public Rigidbody projectileRigidbody;
    public float speed = 100f;
    public Vector3 moveDirection;
    private bool moveInFixedVelocity;

    public void delayDestroy(float delayTime)
    {
        Destroy(gameObject, delayTime);
    }

    public void releaseSpeedControl()
    {
        moveInFixedVelocity = false;
    }

    public void breakConstraint()
    {
        projectileRigidbody.constraints = RigidbodyConstraints.None;
    }

    public void moveProjectile(Vector3 direction)
    {
        //projectileRigidbody.velocity = direction * speed * Time.fixedDeltaTime;
        projectileRigidbody.AddForce(direction * 20f);
    }

    // Start is called before the first frame update
    void Start()
    {
        projectileRigidbody = this.GetComponent<Rigidbody>();
        moveDirection = Vector3.back;
        moveInFixedVelocity = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(moveInFixedVelocity)
            moveProjectile(moveDirection);
    }

    public void OnDestroy()
    {

    }
}
