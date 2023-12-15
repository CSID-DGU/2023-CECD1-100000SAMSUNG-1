using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{




    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void graphStart()
	{
		if (gameObject.activeSelf)
		{
            gameObject.SetActive(false);
		}
		else
		{
            gameObject.SetActive(true);
        }
    }
}
