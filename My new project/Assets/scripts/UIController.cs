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

    // TODO: �Nbutton���W�l�bOnClick()���a��^�ǵ�listener 
    public void clickButton(string buttonName)
    {
        buttonListener?.Invoke(buttonName);
    }

    // Start is called before the first frame update
    void Start()
    {
        // ���button���U��text component
        foreach(Button _button in buttons)
        {
            Text _text = _button.GetComponentInChildren<Text>();
            buttonsTexts.Add(_text);
        }

        // ���U�C�@��button��OnClick listener
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
        //TODO: �bpositionApplyTest��, �Τ@�ӷs����Ƶ��Ulistener,
        //�åB�]�w�ثe��action���O. �z�LEnum.TryParse()�Nbutton string�ഫ��enum
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
