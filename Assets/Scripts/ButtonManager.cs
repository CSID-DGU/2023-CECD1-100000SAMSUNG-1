using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; // UI ���� ���ӽ����̽�

public class ButtonManager : MonoBehaviour
{

    public Button[] buttons; // ��ư �迭
    public Color selectedColor; // ���õ� ��ư�� ��
    private Color defaultColor; // �⺻ ��
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = buttons[0].GetComponent<Image>().color; // ù ��° ��ư�� ���� �⺻ ������ ����

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button)); // �� ��ư�� �̺�Ʈ ������ �߰�
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnButtonClicked(Button clickedButton)
    {
        foreach (var button in buttons)
        {
            SelectExBtn selectedBtn= button.GetComponent<SelectExBtn>();

            selectedBtn.cancelEx();

            button.GetComponent<Image>().color = defaultColor; // ��� ��ư�� �⺻ ������ ����
        }

        clickedButton.GetComponent<Image>().color = Color.cyan; // Ŭ���� ��ư�� �� ����

        SelectExBtn selectBtn = clickedButton.GetComponent<SelectExBtn>();

        selectBtn.selectEx();
    }


}
