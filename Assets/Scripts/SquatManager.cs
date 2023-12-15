using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using TMPro;

public class SquatManager : MonoBehaviour, IManager
{


    public Grib leftGrib;
    public Grib rightGrib;
    public GameObject moveObject;

    private float time = 0f;
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;

    private float veloX = 0f;
    private float veloY = 0f;
    private float veloZ = 0f;

    public static float radius = 0.25f;

    public GameObject model;




    private List<Vector2> accXList;
    private List<Vector2> accYList;
    private List<Vector2> accZList;
    private List<Vector2> timestampList;


    private float distance = 0.0f;

    private float exTime = 10.0f;

    //횟수 측정 변수
    private int exCount = 0;
    private float minDis = 0f;//radius * 0.1f;
    private float maxDis = 0f;// = radius * 0.8f;
    //
    private bool isUp = false;
    private bool isDown = true;

    private bool isReplay = false;

    private List<(float, Vector3)> allPointsLeft;
    private List<(float, Vector3)> allPointsRight;
    private List<(float, Vector3)> allPointsBarbell;



    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        allPointsBarbell = new List<(float, Vector3)>();




        /*        OnFeedbackReceived(1.0f, "속도가 너무 빨라요.");
                OnFeedbackReceived(1.5f, "앞으로 너무 쏠렸어요.");
                OnFeedbackReceived(1.7f, "왼쪽으로 기울었어요.");
                OnFeedbackReceived(2.0f, "속도가 너무 빨라요.");
                OnFeedbackReceived(3.5f, "앞으로 너무 쏠렸어요.");
                OnFeedbackReceived(4.7f, "왼쪽으로 기울었어요.");
                OnFeedbackReceived(5.0f, "속도가 너무 빨라요.");
                OnFeedbackReceived(6.5f, "앞으로 너무 쏠렸어요.");
                OnFeedbackReceived(7.7f, "왼쪽으로 기울었어요.");
                OnFeedbackReceived(8.0f, "속도가 너무 빨라요.");
                OnFeedbackReceived(9.5f, "앞으로 너무 쏠렸어요.");
                OnFeedbackReceived(10.7f, "왼쪽으로 기울었어요.");*/

        initPos = model.transform.position;
        Debug.Log(initPos);
        //초기자세로 이동

        x = initPos.x;

        y = initPos.y;

        z = initPos.z;

        //횟수 측정에 사용될 최대 최소값 설정
        Vector3 tmp = new Vector3(initPos.x, -0.5f + radius, initPos.z);
        minDis = (Vector3.Distance(tmp, initPos)) * 0.4f;
        maxDis = (Vector3.Distance(tmp, initPos)) * 0.6f;
        //StartCoroutine(StartCountdown());

        float val = 0.0f;
        /*        for (float i = 0; i < 10; i += 0.1f)
                {
                    accXList.Add(new Vector2(i * 200, val));
                    val += 5f;
                }*/
    }
    // Update is called once per frame
    void Update()
    {
        //거리값이 변하지 않았다 == 움직이지 않았고, 궤적을 생성하지 않는다.

        //Debug.Log(model.transform.position);

        if (distance != Vector3.Distance(initPos, model.transform.position))
        {
            Vector3 zeroPos = new Vector3(initPos.x, -0.8f, initPos.z);
            distance = Vector3.Distance(zeroPos, model.transform.position);

			Debug.Log("distance , " + distance + model.transform.position);

			// 거리가 지정된 반지름보다 큰 경우 오브젝트의 위치를 수정합니다.
			if (distance > radius)
			{

				// 오브젝트를 구의 표면에 놓아서 원점에서 지정된 반지름만큼 떨어뜨립니다.

				Vector3 fromOriginToObject = model.transform.position - zeroPos;
				fromOriginToObject *= radius / distance; // 거리를 반지름으로 정규화
				Debug.Log("fromOriginToObj ," + fromOriginToObject);
				model.transform.position = zeroPos + fromOriginToObject;

				//범위가 radius 값을 초과하면 값을 끝값에 초기화
/*				x = moveObject.transform.position.x;
				y = moveObject.transform.position.y;
				z = moveObject.transform.position.z;*/

			}

			//지금 나의 위치가 처음 위치보다 낮은값보다 작다면 처음 위치로 이동 
			if (moveObject.transform.position.y < 0f)
			{
             
				Vector3 newPosition = new Vector3(model.transform.position.x, -0.3f, model.transform.position.z);
				model.transform.position = newPosition;
                Debug.Log("B : "+ model.transform.position);
/*				x = moveObject.transform.position.x;
				y = moveObject.transform.position.y;
				z = moveObject.transform.position.z;*/
			}

			//운동 횟수를  측정하는 부분
			distance = Vector3.Distance(zeroPos, model.transform.position);




            if (!isReplay)
            {
                if (distance >= maxDis && isUp)
                {
                    exCount++;
                    isUp = false;
                    isDown = true;

                    if(DataManager.Instance != null)DataManager.Instance.setRTLaps(exCount);

                }
                else if (distance <= minDis && isDown)
                {
                    isDown = false;
                    isUp = true;
                }

                allPointsBarbell.Add((exTime, model.transform.position));

                leftGrib.setTime(exTime);
                rightGrib.setTime(exTime);

                leftGrib.AddPoint();
                rightGrib.AddPoint();
            }


        }


    }


    //dT , time, accX, accY, accZ
    public float[] calculateDis(string v)
    {
        //if (isReplay) return;

        float[] afterDis = new float[7];

        string[] acc_arr = v.Split(",");

        float dT = float.Parse(acc_arr[0]);
        float elapsedTime = float.Parse(acc_arr[1]);
        float accX = float.Parse(acc_arr[2]);
        float accY = float.Parse(acc_arr[3]);
        float accZ = float.Parse(acc_arr[4]);


        //화면에 출력할 시간 갱신
        //exTime = elapsedTime;
        //exTimeText.text = exTime.ToString() + " (초)";

        float threshold = 0.1f;

        if ((accX < threshold && accX > -threshold) && (accY < threshold && accY > -threshold) && (accZ < threshold && accZ > -threshold))
        {
            x += veloX * dT;
            y += veloY * dT;
            z += veloZ * dT;

            veloX = 0f;
            veloY = 0f;
            veloZ = 0f;
        }
        else
        {
            //수직 축인 y 제외한 나머지 축들 값 반영 20%
            x += (veloX * dT + accX * dT * dT / 2) * 0.2f;
            y += (veloY * dT + accY * dT * dT / 2) * 1.5f;
            z += (veloZ * dT + accZ * dT * dT / 2) * 0.2f;

            veloX += accX * dT;
            veloY += accY * dT;
            veloZ += accZ * dT;

            if (veloX >= 1.0)
            {
                veloX = 1.0f;
            }
            else if (veloX <= -1.0)
            {
                veloX = -1.0f;
            }

            if (veloY >= 1.0)
            {
                veloY = 1.0f;
            }
            else if (veloY <= -1.0)
            {
                veloY = -1.0f;
            }

            if (veloZ >= 1.0)
            {
                veloZ = 1.0f;
            }
            else if (veloZ <= -1.0)
            {
                veloZ = -1.0f;
            }
        }


        afterDis[0] = elapsedTime;
        afterDis[1] = accX;
        afterDis[2] = accY;
        afterDis[3] = accZ;
        afterDis[4] = x;
        afterDis[5] = y;
        afterDis[6] = z;

        //excerciseModel(x, y, z);

        return afterDis;
    }

    public void AddPoint()
    {
        leftGrib.AddPoint();
        rightGrib.AddPoint();
    }

    public void ResetPoints()
    {
        leftGrib.ResetPoints();
        rightGrib.ResetPoints();
    }

    public void excerciseModel(float x, float y, float z)
    {
        model.transform.position = new Vector3(x, y, z);
    }

    public Vector3 getInitPos()
    {

        return initPos;
    }

    public void setInitPos()
    {
        model.transform.position = initPos;
    }

    public Vector3 getPos()
    {
        return model.transform.position;
    }

    public void setPos(Vector3 pos)
    {
        model.transform.position = pos;
    }

    public List<(float, Vector3)> getAllBarbellPoints()
    {
        return allPointsBarbell;
    }

    public IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition, float duration)
	{
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            //바벨의 움직임만
            model.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

/*            //손의 움직이도 따라가게
            x = moveObject.transform.position.x;
            y = moveObject.transform.position.y;
            z = moveObject.transform.position.z;*/
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        model.transform.position = endPosition;
    }
}
