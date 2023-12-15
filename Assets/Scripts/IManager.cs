using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IManager 
{
    // Start is called before the first frame update
    float[] calculateDis(string v);
    void AddPoint();
    void ResetPoints();
    void excerciseModel(float x, float y, float z);
    Vector3 getInitPos();
    void setInitPos();
    Vector3 getPos();
    void setPos(Vector3 pos);
    List<(float, Vector3)> getAllBarbellPoints();
    IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition, float duration);
    //string makeFeedbackInfo();
}
