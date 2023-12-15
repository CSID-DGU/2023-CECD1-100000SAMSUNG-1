using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownCollision : MonoBehaviour
{
    private ShoulderManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<ShoulderManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (manager != null)
        {
            if (collision.gameObject.name == "GribPosLeft")
            {
                Debug.Log(collision.gameObject.name);

                manager.setDown();
            }
        }
    }
}
