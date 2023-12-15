using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShoulderManager : MonoBehaviour, IManager
{
    //public GameObject barbel1, barbel2;

    public GameObject moveObject, moveObject2;

    public Grib leftGrib, rightGrib;

    public float x = 0f;
    public float y = 0f;
    public float z =0;

    private float veloX = 0f;
    private float veloY = 0f;
    private float veloZ = 0f;

    public static float radius = 0.23f;

    private List<Vector2> accXList;
    private List<Vector2> accYList;
    private List<Vector2> accZList;
    private List<Vector2> timestampList;

    public TextMeshProUGUI countFlag;

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
    Vector3 initPos2;
    Vector3 tmp;
    // Start is called before the first frame update
    void Start()
    {
        if (DataManager.Instance == null)
        {
            isReplay = false;
        }
        else
        {
            dm = DataManager.Instance;
            isReplay = dm.getIsReplay();
        }

        allPointsBarbell = new List<(float, Vector3)>();
        //Vector3(-0.412999988,1.23699999,0.201000005)
        initPos = moveObject.transform.position;
        initPos2 = moveObject2.transform.position;

        x = initPos.x;

        y = initPos.y;

        z = initPos.z;

        //Debug.Log(initPos+" "+ initPos2);

        tmp = new Vector3(initPos.x, initPos.y - radius, initPos.z);
        /*minDis = (Vector3.Distance(tmp, initPos)) * 0.4f;
        maxDis = (Vector3.Distance(tmp, initPos)) * 0.6f;*/
        minDis = 2 * radius * 0.2f;
        maxDis = 2 * radius * 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("move : " + moveObject.transform.position);

        if (distance != Vector3.Distance(initPos, moveObject.transform.position))
        {
            distance = Vector3.Distance(initPos, moveObject.transform.position);

            //Debug.Log("distance , " + distance);

            // �Ÿ��� ������ ���������� ū ��� ������Ʈ�� ��ġ�� �����մϴ�.
            if (distance > radius)
            {
                // ������Ʈ�� ���� ǥ�鿡 ���Ƽ� �������� ������ ��������ŭ ����߸��ϴ�.
                Vector3 fromOriginToObject = moveObject.transform.position - initPos;
                Vector3 fromOriginToObject2 = moveObject2.transform.position - initPos2;
                fromOriginToObject *= radius / distance; // �Ÿ��� ���������� ����ȭ
                fromOriginToObject2 *= radius / distance; // �Ÿ��� ���������� ����ȭ
                //Debug.Log("fromOriginToObj ," + fromOriginToObject);
                moveObject.transform.position = initPos + fromOriginToObject;
                moveObject2.transform.position = initPos2 + fromOriginToObject2;

				//������ radius ���� �ʰ��ϸ� ���� ������ �ʱ�ȭ
				x = moveObject.transform.position.x;
				y = moveObject.transform.position.y;
				z = moveObject.transform.position.z;

			}

            //���� ���� ��ġ�� ó�� ��ġ���� ���������� �۴ٸ� ó�� ��ġ�� �̵� 
            /*if (moveObject.transform.position.y < initPos.y)
            {
                Vector3 newPosition = new Vector3(moveObject.transform.position.x, initPos.y, moveObject.transform.position.z);
                moveObject.transform.position = newPosition;

                x = moveObject.transform.position.x;
                y = moveObject.transform.position.y;
                z = moveObject.transform.position.z;
            }
*/
            //� Ƚ����  �����ϴ� �κ�

            if(moveObject.transform.position.z < -1.2f)
			{
                Vector3 pos = new Vector3(moveObject.transform.position.x, moveObject.transform.position.y, -1.2f);
                Vector3 pos2= new Vector3(moveObject2.transform.position.x, moveObject2.transform.position.y, -1.2f);
                moveObject.transform.position = pos;
                moveObject2.transform.position = pos2;
                z = -1.2f;
			}

            if (moveObject.transform.position.x > 0.5f)
            {
                Vector3 pos = new Vector3(0.5f, moveObject.transform.position.y, moveObject2.transform.position.z);
                Vector3 pos2 = new Vector3(-0.5f, moveObject2.transform.position.y, moveObject2.transform.position.z);
                moveObject.transform.position = pos;
                moveObject2.transform.position = pos2;
               x = 0.5f;
            }

            if (dm != null) isReplay = dm.getIsReplay();
            if (!isReplay)
            {
                distance = Vector3.Distance(tmp, moveObject.transform.position);

                /*if (distance >= maxDis && isUp)
                {
                    //exCount++;
                    isUp = false;
                    isDown = true;

                    if(DataManager.Instance != null)DataManager.Instance.setRTLaps(++exCount);

                }
                else if (distance <= minDis && isDown)
                {
                    isDown = false;
                    isUp = true;
                }*/

                allPointsBarbell.Add((exTime, moveObject.transform.position));

                leftGrib.setTime(exTime);
                rightGrib.setTime(exTime);

                leftGrib.AddPoint();
                rightGrib.AddPoint();

                //countFlag.text = "isUp : " + isUp + " isDown : " ;
            }


        }
    }

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

        //ZUPT algorithm
        float threshold = 0.23f;
        float yWeight = 1.5f;
        float yVelWeight = 1.0f;

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
            x += (veloX * dT + accX * dT * dT / 2) * 0.1f;

            //�ö󰡰����� ���� �ö󰡴µ� ġ��, �������� ���� ���� �������� ġ��

			if (isUp)
			{
                if((veloY * dT + accY * dT * dT / 2) * yWeight < 0)
				{
                    y+= (veloY * dT + accY * dT * dT / 2) * yWeight * 0.2f;
                }
				else
				{
                    y += (veloY * dT + accY * dT * dT / 2) * yWeight * 1.2f;
                }
            }else if (isDown)
			{
                if ((veloY * dT + accY * dT * dT / 2) * yWeight < 0)
                {
                    y += (veloY * dT + accY * dT * dT / 2) * yWeight * 1.4f;
                }
                else
                {
                    y += (veloY * dT + accY * dT * dT / 2) * yWeight * 0.2f;
                }
            }
            z += (veloZ * dT + accZ * dT * dT / 2) * 0.1f;

            veloX += accX * dT *0.2f;
            veloY += accY * dT;
            veloZ += accZ * dT*0.2f;

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
		/*moveObject.transform.position = initPos + new Vector3(x, y, z);
		moveObject2.transform.position = new Vector3(-initPos.x - x, initPos.y + y, initPos.z + z);*/
		/*barbel1.transform.localPosition = leftGrib.transform.position;
		barbel2.transform.localPosition = rightGrib.transform.position;*/

		moveObject.transform.position = new Vector3(x, y, z);
		moveObject2.transform.position = new Vector3(-x, y, z);
	}

    public Vector3 getInitPos()
    {
        return initPos;
    }

    public void setInitPos()
    {
        moveObject.transform.position = initPos;
        moveObject2.transform.position = initPos2;
        Debug.Log("setInitPos : " + moveObject.transform.position + " , " + initPos);
    }

    public Vector3 getPos()
    {
        return moveObject.transform.position;
    }

    public void setPos(Vector3 pos)
    {
        Vector3 po2 = new Vector3(-pos.x, pos.y, pos.z);
        moveObject.transform.position = pos;
        moveObject2.transform.position = po2;
    }

    public List<(float, Vector3)> getAllBarbellPoints()
    {
        return allPointsBarbell;
    }
    public IEnumerator MoveObject(Vector3 startPosition, Vector3 endPosition, float duration)
    {
        //Debug.Log("MoveObject");
        float elapsedTime = 0;
        Vector3 startPosition2 = new Vector3(-startPosition.x, startPosition.y, startPosition.z);
        Vector3 endPosition2 = new Vector3(-endPosition.x, endPosition.y, endPosition.z);
        while (elapsedTime < duration)
        {
            //Debug.Log("MoveObject While");
            //�ٺ��� �����Ӹ�
            moveObject.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            moveObject2.transform.position = Vector3.Lerp(startPosition2, endPosition2, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        moveObject.transform.position = endPosition;
        moveObject2.transform.position = endPosition2;
    }
    public void setUp()
    {
        isUp = true;
        isDown = false;
    }

    public void setDown()
    {
        if (DataManager.Instance != null && !isDown && isUp) DataManager.Instance.setRTLaps(++exCount);

        isDown = true;
        isUp = false;

    }
}
