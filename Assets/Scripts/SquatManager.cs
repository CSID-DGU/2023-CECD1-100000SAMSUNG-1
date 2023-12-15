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

    //Ƚ�� ���� ����
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




        /*        OnFeedbackReceived(1.0f, "�ӵ��� �ʹ� �����.");
                OnFeedbackReceived(1.5f, "������ �ʹ� ��Ⱦ��.");
                OnFeedbackReceived(1.7f, "�������� �������.");
                OnFeedbackReceived(2.0f, "�ӵ��� �ʹ� �����.");
                OnFeedbackReceived(3.5f, "������ �ʹ� ��Ⱦ��.");
                OnFeedbackReceived(4.7f, "�������� �������.");
                OnFeedbackReceived(5.0f, "�ӵ��� �ʹ� �����.");
                OnFeedbackReceived(6.5f, "������ �ʹ� ��Ⱦ��.");
                OnFeedbackReceived(7.7f, "�������� �������.");
                OnFeedbackReceived(8.0f, "�ӵ��� �ʹ� �����.");
                OnFeedbackReceived(9.5f, "������ �ʹ� ��Ⱦ��.");
                OnFeedbackReceived(10.7f, "�������� �������.");*/

        initPos = model.transform.position;
        Debug.Log(initPos);
        //�ʱ��ڼ��� �̵�

        x = initPos.x;

        y = initPos.y;

        z = initPos.z;

        //Ƚ�� ������ ���� �ִ� �ּҰ� ����
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
        //�Ÿ����� ������ �ʾҴ� == �������� �ʾҰ�, ������ �������� �ʴ´�.

        //Debug.Log(model.transform.position);

        if (distance != Vector3.Distance(initPos, model.transform.position))
        {
            Vector3 zeroPos = new Vector3(initPos.x, -0.8f, initPos.z);
            distance = Vector3.Distance(zeroPos, model.transform.position);

			Debug.Log("distance , " + distance + model.transform.position);

			// �Ÿ��� ������ ���������� ū ��� ������Ʈ�� ��ġ�� �����մϴ�.
			if (distance > radius)
			{

				// ������Ʈ�� ���� ǥ�鿡 ���Ƽ� �������� ������ ��������ŭ ����߸��ϴ�.

				Vector3 fromOriginToObject = model.transform.position - zeroPos;
				fromOriginToObject *= radius / distance; // �Ÿ��� ���������� ����ȭ
				Debug.Log("fromOriginToObj ," + fromOriginToObject);
				model.transform.position = zeroPos + fromOriginToObject;

				//������ radius ���� �ʰ��ϸ� ���� ������ �ʱ�ȭ
/*				x = moveObject.transform.position.x;
				y = moveObject.transform.position.y;
				z = moveObject.transform.position.z;*/

			}

			//���� ���� ��ġ�� ó�� ��ġ���� ���������� �۴ٸ� ó�� ��ġ�� �̵� 
			if (moveObject.transform.position.y < 0f)
			{
             
				Vector3 newPosition = new Vector3(model.transform.position.x, -0.3f, model.transform.position.z);
				model.transform.position = newPosition;
                Debug.Log("B : "+ model.transform.position);
/*				x = moveObject.transform.position.x;
				y = moveObject.transform.position.y;
				z = moveObject.transform.position.z;*/
			}

			//� Ƚ����  �����ϴ� �κ�
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


        //ȭ�鿡 ����� �ð� ����
        //exTime = elapsedTime;
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
            //�ٺ��� �����Ӹ�
            model.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);

/*            //���� �����̵� ���󰡰�
            x = moveObject.transform.position.x;
            y = moveObject.transform.position.y;
            z = moveObject.transform.position.z;*/
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        model.transform.position = endPosition;
    }
}
