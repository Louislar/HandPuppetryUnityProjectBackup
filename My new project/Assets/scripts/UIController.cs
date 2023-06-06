using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIController : MonoBehaviour
{
    public Text actionPanelText;
    public List<Button> buttons;
    public List<Text> buttonsTexts;
    public string curBtnActionName;
    public string curBtnText;
    public event Action<string> buttonListener;

    // TODO: 將button的名子在OnClick()的地方回傳給listener 
    public void clickButton(string buttonName)
    {
        buttonListener?.Invoke(buttonName);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 找到button底下的text component
        foreach(Button _button in buttons)
        {
            Text _text = _button.GetComponentInChildren<Text>();
            buttonsTexts.Add(_text);
        }

        // 註冊每一個button的OnClick listener
        for(int i=0; i<buttons.Count;++i)
        {
            string _buttonName = buttons[i].name;
            string _buttonText = buttonsTexts[i].text;
            print(i);
            buttons[i].onClick.AddListener(() => {
                clickButton(_buttonName);
                actionPanelText.text = _buttonText;
                
            });
        }
        //TODO: 在positionApplyTest當中, 用一個新的函數註冊listener,
        //並且設定目前的action類別. 透過Enum.TryParse()將button string轉換成enum
    }

    // Update is called once per frame
    void Update()
    {
        for(int i=0; i<5;++i)
        {
            if (Input.GetKeyDown(i + KeyCode.Alpha1))
            {
                buttons[i].onClick.Invoke();
                //print($"Number {i + KeyCode.Alpha1} key was pressed");
            }
        }
    }
}
