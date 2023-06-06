using System.Collections;
using UnityEngine;

public class SelectableCharacter : MonoBehaviour {

    public SpriteRenderer selectImage;
    public bool isRotate;
    public bool isMoving;


    private void Awake() {
        //selectImage.enabled = false;
    }

    //Turns off the sprite renderer
    public void TurnOffSelector()
    {
        selectImage.enabled = false;
    }

    //Turns on the sprite renderer
    public void TurnOnSelector()
    {
        selectImage.enabled = true;
    }

    public IEnumerator movingUpAndDown()
    {
        float upperLimit = transform.position.y + 0.5f;
        float LowerLimit = transform.position.y - 0.5f;
        float desiredYPos = transform.position.y + 0.01f;
        float movingDir = 0.01f;
        float speed = 50;
        
        while(true)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, desiredYPos, transform.position.z), Time.deltaTime * speed);
            if (Mathf.Abs(desiredYPos - transform.position.y) < 0.001f)
                desiredYPos += movingDir;
            if (Mathf.Abs(LowerLimit - transform.position.y) < 0.001f)
            {
                movingDir = 0.01f;
            }
            else if (Mathf.Abs(upperLimit - transform.position.y) < 0.001f)
            {
                movingDir =  - 0.01f;
            }
            yield return null;
        }
        //this.transform.position;
    }

    /// <summary>
    /// 不斷繞著Y軸旋轉360度 
    /// </summary>
    IEnumerator rotateAlongY360()
    {
        float desiredYRot = transform.rotation.eulerAngles.y + 1;
        float speed = 100;
        while(true)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, desiredYRot, transform.rotation.eulerAngles.z), Time.deltaTime * speed);
            if (Mathf.Abs(desiredYRot - transform.rotation.eulerAngles.y) < 0.001f)
                desiredYRot = desiredYRot+1;
            if (desiredYRot == 360)
                desiredYRot = 0f;
            //print(desiredYRot);
            //print(transform.rotation.eulerAngles.y);
            //print(desiredYRot - transform.rotation.eulerAngles.y);
            yield return null;
        }
        yield return null;
    }

    public void Start()
    {
        if(isRotate)
            StartCoroutine(rotateAlongY360());
        if(isMoving)
            StartCoroutine(movingUpAndDown());
    }
}
