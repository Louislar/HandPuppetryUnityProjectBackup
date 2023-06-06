using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintController : MonoBehaviour
{
    public Text hintText;
    // public string changeHintTextSrc;
    public List<SelectableCharacter> hintScripts;
    public TextAsset changeHintTextSrc;

    /// <summary>
    /// ������triggerĲ�o, �N�i�J��U�@�Ӷ��q
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        print("=====");
        print(other.transform.name);
        changeStage();
    }

    /// <summary>
    /// �ܧ�C�����q 
    /// �ݭn�����ثe��Hint arrow�H�Φa�O����
    /// �ݭn�ܧ�Hint text�����e
    /// </summary>
    public void changeStage()
    {
        foreach(SelectableCharacter sc in hintScripts)
        {
            sc.TurnOffSelector();
        }
        changeHintText();
    }


    /// <summary>
    /// �ܧ�Hint text�����e, �s�����e�ӷ��O.txt�ɮ�
    /// </summary>
    public void changeHintText()
    {
        hintText.text = changeHintTextSrc.text;
    }

    // Start is called before the first frame update
    void Start()
    {
        // �hlistener���Uchange stage 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
