using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI; // UI 관련 네임스페이스

public class ButtonManager : MonoBehaviour
{

    public Button[] buttons; // 버튼 배열
    public Color selectedColor; // 선택된 버튼의 색
    private Color defaultColor; // 기본 색
    // Start is called before the first frame update
    void Start()
    {
        defaultColor = buttons[0].GetComponent<Image>().color; // 첫 번째 버튼의 색을 기본 색으로 설정

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button)); // 각 버튼에 이벤트 리스너 추가
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

            button.GetComponent<Image>().color = defaultColor; // 모든 버튼을 기본 색으로 변경
        }

        clickedButton.GetComponent<Image>().color = Color.cyan; // 클릭된 버튼의 색 변경

        SelectExBtn selectBtn = clickedButton.GetComponent<SelectExBtn>();

        selectBtn.selectEx();
    }


}
