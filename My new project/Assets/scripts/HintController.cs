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
    /// 偵測有trigger觸發, 就進入到下一個階段
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        print("=====");
        print(other.transform.name);
        changeStage();
    }

    /// <summary>
    /// 變更遊戲階段 
    /// 需要關閉目前的Hint arrow以及地板光圈
    /// 需要變更Hint text的內容
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
    /// 變更Hint text的內容, 新的內容來源是.txt檔案
    /// </summary>
    public void changeHintText()
    {
        hintText.text = changeHintTextSrc.text;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 去listener註冊change stage 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
