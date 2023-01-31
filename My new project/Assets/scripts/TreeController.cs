using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public bool isShaking;
    public GameObject appleGO;

    public void dropApple()
    {
        // TODO 
    }

    public void shakeTree()
    {
        if (isShaking == false)
        {
            isShaking = true;
            StartCoroutine(ShakeObject(1f));
        }
        
    }

    IEnumerator ShakeObject(float duration)
    {
        int direction = 1;
        float elapsedTime = 0;
        float timer = 0;
        const float toggleTime = 0.1f; // change direction every 0.1s
        const float maxAngle = 3;
        while (elapsedTime <= duration)
        {
            elapsedTime += Time.deltaTime;
            timer += Time.deltaTime;
            float zAngle = direction * timer * (maxAngle / toggleTime);
            this.transform.rotation = Quaternion.Euler(0, 0, zAngle);
            if (timer >= toggleTime)
            {
                direction = direction * -1;
                timer = 0;
            }
            yield return null;
        }
        isShaking = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //shakeTree();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
