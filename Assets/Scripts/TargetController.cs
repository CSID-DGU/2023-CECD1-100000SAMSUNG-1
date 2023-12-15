using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public Transform barbelGrib;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(this.gameObject.name);
    }

    // Update is called once per frame
    void Update()
	{
		//Debug.Log(this.gameObject.name + " " + this.transform.position);
		
        this.transform.position = barbelGrib.transform.position;
    }
}
