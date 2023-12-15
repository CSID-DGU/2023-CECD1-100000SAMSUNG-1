using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquirtController : MonoBehaviour
{
    public float x,y,z;

    public GameObject model;

    private void Awake() {
        x = model.transform.position.x;
        y = model.transform.position.y;
        z = model.transform.position.z;
    }
    private void Update() {
        model.transform.position = new Vector3(x,y,z);
    }
}
