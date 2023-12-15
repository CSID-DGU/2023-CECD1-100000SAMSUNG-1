using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackSelectEx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void moveScene()
    {
        SceneManager.LoadScene("Excercise");
        DataManager.Instance.setExName("");
        DataManager.Instance.setExLaps(0);
        DataManager.Instance.setExWeight(0);
    }
}
