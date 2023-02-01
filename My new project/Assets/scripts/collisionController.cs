using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionController : MonoBehaviour
{
    public legCollision legCollisionOB;
    public List<TreeController> treeControllerOBs;
    public ProjectileController projectileController;


    // Start is called before the first frame update
    void Start()
    {
        foreach(TreeController _controller in treeControllerOBs)
        {
            legCollisionOB.LegCollisionEvent += _controller.shakeTree;
            legCollisionOB.LegCollisionEvent += _controller.dropApple;
        }
        foreach(ProjectileObject projectileOB in projectileController.projectiles)
        {
            //legCollisionOB.LegCollideEvent += (x) => projectileOB.breakConstraint();
            //legCollisionOB.LegCollideEvent += (x) => projectileOB.setSpeedToZero();
        }
        legCollisionOB.LegCollideEvent += projectileController.OnCollideProjectile;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
