using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;
using TMPro;

public class Manager : MonoBehaviour
{
    public GameObject benchModel;
    public GameObject squatModel;
    public GameObject shoulderModel;
    //public GameObject benchModel;
    //public GameObject benchModel;

    private GameObject targetModel;

    private IManager targetManager ;

    public GameObject explainUI;

    public BenchManager bm;
    public SquatManager sm;
    public ShoulderManager shm;

    private float time = 0f;
    public float x = 0f;
    public float y = 0f;
    public float z = 0f;

    private float veloX = 0f;
    private float veloY = 0f;
    private float veloZ = 0f;

    public static float radius = 0.6f;

    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI exTimeText;
    public TextMeshProUGUI feedbackText;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI exName;
    public TextMeshProUGUI countFlag;
    public TextMeshProUGUI explainInfo;


    public List<(double,string)> feedback;
    public GameObject feebackPrefab;
    public Transform feedbackContentPanel;

    public GameObject buttonPrefab; // ��ư ������
    public Transform buttonsParent; // ��ư���� ���� �θ� ��ü
    private List<GameObject> buttons;

    public Button playButton;
    public Button stopButton;
    public Slider replaySlider;
    public GameObject replayUI;
    public Button endButton;

    public UILineRenderer accXLine;
    public UILineRenderer accYLine;
    public UILineRenderer accZLine;
    public UILineRenderer timestamp;

    private List<Vector2> accXList;
    private List<Vector2> accYList;
    private List<Vector2> accZList;
    private List<Vector2> timestampList;

    public RectTransform viewport;
    public RectTransform feedbackViewport;

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

    private bool isReplay = false;

    private int currentIndex = 0;
    private bool isPlaying = false;
    private IEnumerator replayCoroutine;

    private GameObject moveObject;

    Vector3 initPos;
    // Start is called before the first frame update
    void Start()
    {
        benchModel.gameObject.SetActive(false);
        squatModel.gameObject.SetActive(false);
        shoulderModel.gameObject.SetActive(false); 
        if (DataManager.Instance == null)
        {
            exName.text = "����Ʈ";
            targetManager = shm;
            targetModel = shoulderModel;
            shoulderModel.gameObject.SetActive(true);
            explainInfo.text = "";
        }
        else
        {
            explainInfo.text = DataManager.Instance.getExplainInfo();
            exName.text = DataManager.Instance.getExName();
            if(exName.text == "Benchpress")
			{
                targetManager = bm;
                targetModel = benchModel;
                benchModel.gameObject.SetActive(true);
            }
            else if(exName.text == "Squat")
			{
                targetManager = sm;
                targetModel = squatModel;
                squatModel.gameObject.SetActive(true);
			}
            else if (exName.text == "Shoulderpress")
            {
                targetManager = shm;
                targetModel = shoulderModel;
                shoulderModel.gameObject.SetActive(true);
            }
            else
			{
                targetManager = shm;
                targetModel = shoulderModel;
                shoulderModel.gameObject.SetActive(true);
            }
        }



        feedback = new List<(double, string)>();
        buttons = new List<GameObject>();

        allPointsLeft = new List<(float, Vector3)>();
        allPointsRight = new List<(float, Vector3)>();
        allPointsBarbell = new List<(float, Vector3)>();

        accXList = new List<Vector2>();
        accYList = new List<Vector2>();
        accZList = new List<Vector2>();
        timestampList = new List<Vector2>();

        buttonsParent.gameObject.SetActive(false);
        playButton.gameObject.SetActive(false);
        stopButton.gameObject.SetActive(false);
        replaySlider.gameObject.SetActive(false);
        replayUI.gameObject.SetActive(false);




		/*OnFeedbackReceived(1.0f, "�ӵ��� �ʹ� �����.");
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
		OnFeedbackReceived(10.7f, "�������� �������.");
        OnFeedbackReceived(12.7f, "�������� �������.");
        OnFeedbackReceived(13.7f, "�������� �������.");*/

        initPos = targetManager.getInitPos();

        //�ʱ��ڼ��� �̵�

/*        x = initPos.x;

        y = initPos.y;

        z = initPos.z;*/

        //Ƚ�� ������ ���� �ִ� �ּҰ� ����
        Vector3 tmp = new Vector3(initPos.x, initPos.y + radius, initPos.z);
        minDis = (Vector3.Distance(tmp, initPos)) * 0.4f;
        maxDis = (Vector3.Distance(tmp, initPos)) * 0.6f;
		
		float val = 0.0f;
/*		for (float i = 0; i < 10; i += 0.1f)
		{
			accXList.Add(new Vector2(i*200, val));
			val += 5f;
		}*/

        //targetManager.setInitPos();
	}

    public void closeExplain()
	{
        explainUI.gameObject.SetActive(false);

        StartCoroutine(StartCountdown());
	}

    private IEnumerator StartCountdown()
    {
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();  // ī��Ʈ�ٿ� �ؽ�Ʈ�� ������Ʈ�մϴ�.
            yield return new WaitForSeconds(1);  // 1�� ���� ����մϴ�.
        }

        countdownText.text = "";  // ī��Ʈ�ٿ� �ؽ�Ʈ�� ���ϴ�.
    }

    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance != null) countFlag.text = replaySlider.value.ToString() + " / " + replaySlider.maxValue;

        if (DataManager.Instance == null)
        {

        }
		else
		{
            exName.text = DataManager.Instance.getExName();
            countText.text = DataManager.Instance.getRTLaps().ToString() + " (ȸ)";
			//exTimeText.text = DataManager.Instance.getRTTime().ToString() + " (��)";
		}
        //Debug.Log("xyz : "+ x+ y+ z);
		if(!isReplay) targetManager.excerciseModel(x, y, z);

	}

    //dT , time, accX, accY, accZ
    public void calculateDis(string v)
    {
        if (isReplay) return;

        string[] acc_arr = v.Split(",");

        float dT = float.Parse(acc_arr[0]);
        float elapsedTime = float.Parse(acc_arr[1]);
        float accX = float.Parse(acc_arr[2]);
        float accY = float.Parse(acc_arr[3]);
        float accZ = float.Parse(acc_arr[4]);

        Vector2 accXVec = new Vector3(elapsedTime * 50f, accX * 50f);
        Vector2 accYVec = new Vector3(elapsedTime * 50f, accY * 50f);
        Vector2 accZVec = new Vector3(elapsedTime * 50f, accZ * 50f);
        Vector2 timestamp = new Vector3(elapsedTime * 50f, 0f);

        accXList.Add(accXVec);
        accYList.Add(accYVec);
        accZList.Add(accZVec);
        timestampList.Add(timestamp);

        float[] afterDis;

        afterDis = targetManager.calculateDis(v);

		x = afterDis[4];
		y = afterDis[5];
		z = afterDis[6];

        exTimeText.text = elapsedTime.ToString();
        if (DataManager.Instance != null) DataManager.Instance.setRTTime(elapsedTime);


		makeFeedback(afterDis[0], afterDis[1], afterDis[2],afterDis[3]);

        
    }

    public void makeFeedback(float time,float accX, float accY, float accZ)
    {
        //���ذ��� ���ӵ� ���� üŷ�Ѵ�.
        //���ذ� �ʰ��� �ش� ���ӵ��� �ð����� �ǵ�� ������ ����Ʈȭ�Ѵ�.

        float maxX = 1.5f;
        float minX = -1.5f;
        float maxY = 2.5f;
        float minY = -2.5f;
        float maxZ = 1.5f;
        float minZ = -1.5f;

        if(accX > maxX)
        {
            OnFeedbackReceived(time, "�� ������ �־����ϴ�.");
        }
        else if (accX < minX)
        {
            OnFeedbackReceived(time, "�� �̶� �ʹ� �������ϴ�.");
        }
        if (accY > maxY)
        {
            OnFeedbackReceived(time, "�ö���� �ӵ��� �����ϴ�.");
        }
        else if (accY < minY)
        {
            OnFeedbackReceived(time, "�������� �ӵ��� �����ϴ�.");
        }
        if (accZ > maxZ)
        {
            OnFeedbackReceived(time, "�������� �򸳴ϴ�.");
        }
        else if (accZ < minZ)
        {
            OnFeedbackReceived(time, "���������� �򸳴ϴ�.");
        }



    }



    public void OnFeedbackReceived(double time, string text)
    {

        StartCoroutine(CreateButtonAfterDelay(time, text));
    }

    // ������ �ð���ŭ ����� �� ��ư�� �����ϴ� �ڷ�ƾ
    IEnumerator CreateButtonAfterDelay(double delay, string text)
    {
        //yield return new WaitForSeconds((float)delay);

        TextMeshProUGUI[] existingTexts = buttonsParent.GetComponentsInChildren<TextMeshProUGUI>();
        float time = Mathf.Floor((float)delay);
        string delayText = time.ToString();

        feedback.Add((time, text));

        feedbackText.text = "";

        if(feedbackContentPanel.transform.childCount > 5)
		{
           Destroy(feedbackContentPanel.transform.GetChild(0).gameObject);   
        }

        feedback = feedback.Distinct().ToList();

        for (int i = 0; i < feedback.Count; i++)
        {
            if (DataManager.Instance != null)
			{
                if (feedback[i].Item1 == Mathf.Floor(DataManager.Instance.getRTTime()))
                {
                    //feedbackText.text += feedback[i].Item2 + "\n";

                    GameObject newFeedback = Instantiate(feebackPrefab, feedbackContentPanel);
                    TextMeshProUGUI[] textComponents = newFeedback.GetComponentsInChildren<TextMeshProUGUI>();

                    textComponents[1].text = feedback[i].Item1.ToString();
                    textComponents[0].text = text;


                }
            }
			else
			{
                
                    feedbackText.text += feedback[i].Item2 + "\n";

                    GameObject newFeedback = Instantiate(feebackPrefab, feedbackContentPanel);
                    TextMeshProUGUI[] textComponents = newFeedback.GetComponentsInChildren<TextMeshProUGUI>();

                    textComponents[1].text = feedback[i].Item1.ToString();
                    textComponents[0].text = text;


                
            }
        }

		/*while (feedbackContentPanel.childCount > 5)
		{
			Destroy(feedbackContentPanel.transform.GetChild(0).gameObject);
		}*/

		Debug.Log("feedback Panel length : " + feedbackContentPanel.transform.childCount);


        foreach (var existingText in existingTexts)
        {
            if (existingText.text == delayText)
            {
                yield break; // �̹� �����ϸ� �ڷ�ƾ�� �����մϴ�.
            }
        }


        // ��ư ���� ����
        GameObject buttonObj = Instantiate(buttonPrefab, buttonsParent);
        TextMeshProUGUI textComponent = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = time.ToString();
        }
        else
        {
            Debug.LogError("Text component not found on the button prefab!");
        }

        // ��ư�� Button ������Ʈ�� Ŭ�� �����ʸ� �߰��մϴ�.
        Button buttonComponent = buttonObj.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => ButtonClicked(time, text));
        }
        else
        {
            Debug.LogError("Button component not found on the button prefab!");
        }

        // ��ư�� ������ ���� �߰����� ����
        buttons.Add(buttonObj);
    }

    // ��ư Ŭ���� ȣ��� �޼ҵ�
    void ButtonClicked(double time, string text)
    {
        feedbackText.text = "";
        // ���⿡ �ǵ�鿡 ���� ó�� ������ �����մϴ�.
        int count = 0;
        foreach (Transform feedback in feedbackContentPanel.transform)
        {
            GameObject.Destroy(feedback.gameObject);
        }
        for (int i = 0; i < feedback.Count; i++)
        {
            //Debug.Log(feedback[i].Item1+", "+time.ToString());



            if(feedback[i].Item1 == time)
            {
                GameObject newFeedback = Instantiate(feebackPrefab, feedbackContentPanel);
                TextMeshProUGUI[] textComponents = newFeedback.GetComponentsInChildren<TextMeshProUGUI>();

                textComponents[1].text = (count+1).ToString();
                textComponents[0].text = text;
                count++;

            }

            
        }

        //�ش� �ʿ� �´� �ε����� ã�� ���÷��� ����
        for(int i = 0; i < allPointsBarbell.Count; i++)
		{
            if(time <= allPointsBarbell[i].Item1)
			{
                currentIndex = i;
                break;
			}
		}

        replaySlider.value = allPointsBarbell[currentIndex].Item1;

        //StartReplay();
    }

    public void endExcercise (string v)
    {
        isReplay = true;

        targetManager.ResetPoints();
        if (DataManager.Instance != null) DataManager.Instance.setIsReplay(true);

        //�ʱ��ڼ��� �̵�

        Vector3 initV = targetManager.getInitPos();

        x = initV.x;
        y = initV.y;
        z = initV.z;

        targetManager.setInitPos();
        
        //�ð��� ��ư�� ���̱�
        buttonsParent.gameObject.SetActive(true);
        //���÷��� ��� ���̱�
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(true);
        replaySlider.gameObject.SetActive(true);
        replayUI.gameObject.SetActive(true);
        endButton.gameObject.SetActive(false);

        if(DataManager.Instance != null)replaySlider.maxValue = DataManager.Instance.getRTTime();

        //�׷��� �����
        allPointsBarbell = targetManager.getAllBarbellPoints();

        viewport.sizeDelta = new Vector2(accXList[accXList.Count-1].x, viewport.sizeDelta.y);

        

        accXLine.rectTransform.sizeDelta = new Vector2(accXList.Count, 100);
        accXLine.points = accXList.ToArray();

        accYLine.rectTransform.sizeDelta = new Vector2(accYList.Count, 100);
        accYLine.points = accYList.ToArray();

        accZLine.rectTransform.sizeDelta = new Vector2(accZList.Count, 100);
        accZLine.points = accZList.ToArray();

        timestamp.rectTransform.sizeDelta = new Vector2(timestampList.Count, 100);
        timestamp.points = timestampList.ToArray();

        foreach (Transform feedback in feedbackContentPanel.transform)
        {
            GameObject.Destroy(feedback.gameObject);
        }

    }

    public void StartReplay()
    {
        Debug.Log("StartReplay");
        targetManager.ResetPoints();

		if (!isPlaying)
		{
            replayCoroutine = playReplay();
            StartCoroutine(replayCoroutine);
        }
		else
		{
            replayCoroutine = playReplay();
            StopCoroutine(replayCoroutine);
            StartCoroutine(replayCoroutine);
		}
    }

    public void StopReplay()
    {
        Debug.Log("StopReplay");
        if (isPlaying && replayCoroutine != null)
        {
            Debug.Log("stop replay");
            StopCoroutine(replayCoroutine);
            isPlaying = false;
        }
    }

	IEnumerator playReplay()
	{
        Debug.Log("Start");
        isPlaying = true;

        replaySlider.value = allPointsBarbell[currentIndex].Item1;

		for (int i = currentIndex; i < allPointsBarbell.Count - 1; i++)
		{
			currentIndex = i;
			Vector3 startPosition = new Vector3(allPointsBarbell[i].Item2.x, allPointsBarbell[i].Item2.y, allPointsBarbell[i].Item2.z);
			Vector3 endPosition = new Vector3(allPointsBarbell[i + 1].Item2.x, allPointsBarbell[i + 1].Item2.y, allPointsBarbell[i + 1].Item2.z);

			// MoveObject �ڷ�ƾ�� ȣ���Ͽ� �� �������� õõ�� �̵�
			if (i != 0)
			{
				yield return StartCoroutine(targetManager.MoveObject(startPosition, endPosition, allPointsBarbell[i].Item1 - allPointsBarbell[i - 1].Item1)); // 1.0f�� �̵��� �ɸ��� �ð�, �ʿ信 ���� �����ϼ���.
				replaySlider.value += allPointsBarbell[i].Item1 - allPointsBarbell[i - 1].Item1;

			}
			else
			{
				yield return StartCoroutine(targetManager.MoveObject(startPosition, endPosition, allPointsBarbell[i].Item1));
				replaySlider.value += allPointsBarbell[i].Item1;
			}

            targetManager.AddPoint();

		}
		isPlaying = false;

		replaySlider.value = 0.0f;

        targetManager.ResetPoints();

		currentIndex = 0;
        targetManager.setInitPos();
	}

	public void moveScene()
    {
        if(DataManager.Instance == null)
		{

		}
		else
		{
            DataManager.Instance.reset();
        }
        SceneManager.LoadScene("selectEx");
    }


    public void test()
	{
        Vector3 test = targetManager.getPos();
        x = test.x + 0.01f;
        y = test.y - 0.00f;
        z = test.z + 0.01f;

        Vector3 v = new Vector3(x, y, z);
        targetManager.setPos(v);
        Debug.Log("test : "+x+" : "+y+" : " + z);

    }

    
    
}
 