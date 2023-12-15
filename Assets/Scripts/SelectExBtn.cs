using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectExBtn : MonoBehaviour
{

    public GameObject laps;
    public GameObject weight;

    private bool isSelected = false;
    //private Renderer btnColor;
    private Image btnColor;
    // Start is called before the first frame update
    void Start()
    {
        laps.gameObject.SetActive(false);
        weight.gameObject.SetActive(false);

    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectEx()
	{
        this.gameObject.GetComponent<Image>().color = Color.cyan;
        DataManager.Instance.setExName( this.gameObject.name);
        laps.gameObject.SetActive(true);
        weight.gameObject.SetActive(true);
    }

    public void cancelEx()
	{
        laps.gameObject.SetActive(false);
        weight.gameObject.SetActive(false);
    }
}
