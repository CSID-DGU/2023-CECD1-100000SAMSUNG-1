using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using TMPro;

public class BenchManager : MonoBehaviour,IManager
{


    public Grib leftGrib;
    public RightGrib rightGrib;
    public GameObject moveObject;

    private float time = 0f;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    private float veloX = 0f;
    private float veloY = 0f;
    private float veloZ = 0f;

    public static float radius = 0.6f;

    private List<Vector2> accXList;
    private List<Vector2> accYList;
    private List<Vector2> accZList;
    private List<Vector2> timestampList;

    private float distance = 0.0f;

    private float exTime = 10.0f;

    //Ƚ�� ���� ����
    private int exCount = 0;
    private float minDis = 0f;//radius * 0.1f;
    private float maxDis = 0f;// = radius * 0.8f;
    //
    private bool isUp = true;
    private bool isDown = false;

    private List<(float, Vector3)> allPointsLeft;
    private List<(float, Vector3)> allPointsRight;
    private List<(float, Vector3)> allPointsBarbell;

    private bool isReplay;
    private DataManager dm;
    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {

        if(DataManager.Instance == null)
		{
            isReplay = false;
		}
		else
		{
            dm = DataManager.Instance;
            isReplay = dm.getIsReplay();
		}

        allPointsBarbell = new List<(float, Vector3)>();

        initPos = moveObject.transform.position;

        //�ʱ��ڼ��� �̵�

        x = initPos.x;

        y = initPos.y;

        z = initPos.z;

        //Ƚ�� ������ ���� �ִ� �ּҰ� ����
        Vector3 tmp = new Vector3(initPos.x, initPos.y + radius, initPos.z);
        minDis = (Vector3.Distance(tmp, initPos)) * 0.4f;
        maxDis = (Vector3.Distance(tmp, initPos)) * 0.6f;

    }
    // Update is called once per frame
    void Update()
    {


        //moveObject.transform.position = new Vector3(x, y, z);

        //�Ÿ����� ������ �ʾҴ� == �������� �ʾҰ�, ������ �������� �ʴ´�.
        if (distance != Vector3.Distance(initPos, moveObject.transform.position))
        {
            distance = Vector3.Distance(initPos, moveObject.transform.position);

            //Debug.Log("distance , " + distance);

            // �Ÿ��� ������ ���������� ū ��� ������Ʈ�� ��ġ�� �����մϴ�.
            if (distance > radius)
            {
                // ������Ʈ�� ���� ǥ�鿡 ���Ƽ� �������� ������ ��������ŭ ����߸��ϴ�.
                Vector3 fromOriginToObject = moveObject.transform.position - initPos;
                fromOriginToObject *= radius / distance; // �Ÿ��� ���������� ����ȭ
                Debug.Log("fromOriginToObj ," + fromOriginToObject);
                moveObject.transform.position = initPos + fromOriginToObject;

                //������ radius ���� �ʰ��ϸ� ���� ������ �ʱ�ȭ
                x = moveObject.transform.position.x;
                y = moveObject.transform.position.y;
                z = moveObject.transform.position.z;

            }

            //���� ���� ��ġ�� ó�� ��ġ���� ���������� �۴ٸ� ó�� ��ġ�� �̵� 
            if (moveObject.transform.position.y < initPos.y)
            {
                Vector3 newPosition = new Vector3(moveObject.transform.position.x, initPos.y, moveObject.transform.position.z);
                moveObject.transform.position = newPosition;

                x = moveObject.transform.position.x;
                y = moveObject.transform.position.y;
                z = moveObject.transform.position.z;
            }

            //� Ƚ����  �����ϴ� �κ�
            


			if (!isReplay)
			{
                distance = Vector3.Distance(initPos, moveObject.transform.position);

                if (distance >= maxDis && isUp)
                {
                    exCount++;
                    isUp = false;
                    isDown = true;

                    DataManager.Instance.setRTLaps(exCount);

                }
                else if (distance <= minDis && isDown)
                {
                    isDown = false;
                    isUp = true;
                }


                allPointsBarbell.Add((exTime, moveObject.transform.position));

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
        float[] afterDis = new float[7];
        //if (isReplay) return;
        string[] acc_arr = v.Split(",");

        float dT = float.Parse(acc_arr[0]);
        float elapsedTime = float.Parse(acc_arr[1]);
        float accX = float.Parse(acc_arr[2]);
        float accY = float.Parse(acc_arr[3]);
        float accZ = float.Parse(acc_arr[4]);

        //ȭ�鿡 ����� �ð� ����
        exTime = elapsedTime;
        //exTimeText.text = exTime.ToString() + " (��)";

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
            //���� ���� y ������ ������ ��� �� �ݿ� 20%
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
        //makeFeedback(elapsedTime, accX, accY, accZ);


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
        moveObject.transform.position = new Vector3(x, y, z);
    }

    public Vector3 getInitPos()
    {
        return initPos;
    }

    public void setInitPos()
	{
        moveObject.transform.position = initPos;
        Debug.Log("setInitPos : " + moveObject.transform.position + " , " + initPos);
    }

    public Vector3 getPos()
    {
        return moveObject.transform.position;
    }

    public void setPos(Vector3 pos)
    {
        moveObject.transform.position = pos;
    }

    public List<(float,Vector3)> getAllBarbellPoints()
	{
        return allPointsBarbell;
	}
    public IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition, float duration)
	{
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            //�ٺ��� �����Ӹ�
            moveObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

			//���� �����̵� ���󰡰�
			x = moveObject.transform.position.x;
			y = moveObject.transform.position.y;
			z = moveObject.transform.position.z;
			elapsedTime += Time.deltaTime;
            yield return null;
        }

        moveObject.transform.position = endPosition;
    }
}
