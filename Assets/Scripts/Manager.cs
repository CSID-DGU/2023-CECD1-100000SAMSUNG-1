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

    public GameObject buttonPrefab; // 버튼 프리팹
    public Transform buttonsParent; // 버튼들을 담을 부모 객체
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

    //횟수 측정 변수
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
            exName.text = "스쿼트";
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




		/*OnFeedbackReceived(1.0f, "속도가 너무 빨라요.");
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
		OnFeedbackReceived(10.7f, "왼쪽으로 기울었어요.");
        OnFeedbackReceived(12.7f, "왼쪽으로 기울었어요.");
        OnFeedbackReceived(13.7f, "왼쪽으로 기울었어요.");*/

        initPos = targetManager.getInitPos();

        //초기자세로 이동

/*        x = initPos.x;

        y = initPos.y;

        z = initPos.z;*/

        //횟수 측정에 사용될 최대 최소값 설정
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
            countdownText.text = i.ToString();  // 카운트다운 텍스트를 업데이트합니다.
            yield return new WaitForSeconds(1);  // 1초 동안 대기합니다.
        }

        countdownText.text = "";  // 카운트다운 텍스트를 비웁니다.
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
            countText.text = DataManager.Instance.getRTLaps().ToString() + " (회)";
			//exTimeText.text = DataManager.Instance.getRTTime().ToString() + " (초)";
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
        //기준값과 가속도 값을 체킹한다.
        //기준값 초과시 해당 가속도의 시간값과 피드백 문구를 리스트화한다.

        float maxX = 1.5f;
        float minX = -1.5f;
        float maxY = 2.5f;
        float minY = -2.5f;
        float maxZ = 1.5f;
        float minZ = -1.5f;

        if(accX > maxX)
        {
            OnFeedbackReceived(time, "몸 쪽으로 멀어집니다.");
        }
        else if (accX < minX)
        {
            OnFeedbackReceived(time, "몸 이랑 너무 가깝습니다.");
        }
        if (accY > maxY)
        {
            OnFeedbackReceived(time, "올라오는 속도가 빠릅니다.");
        }
        else if (accY < minY)
        {
            OnFeedbackReceived(time, "내려가는 속도가 빠릅니다.");
        }
        if (accZ > maxZ)
        {
            OnFeedbackReceived(time, "왼쪽으로 쏠립니다.");
        }
        else if (accZ < minZ)
        {
            OnFeedbackReceived(time, "오른쪽으로 쏠립니다.");
        }



    }



    public void OnFeedbackReceived(double time, string text)
    {

        StartCoroutine(CreateButtonAfterDelay(time, text));
    }

    // 지정된 시간만큼 대기한 후 버튼을 생성하는 코루틴
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
                yield break; // 이미 존재하면 코루틴을 종료합니다.
            }
        }


        // 버튼 생성 로직
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

        // 버튼의 Button 컴포넌트에 클릭 리스너를 추가합니다.
        Button buttonComponent = buttonObj.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.onClick.AddListener(() => ButtonClicked(time, text));
        }
        else
        {
            Debug.LogError("Button component not found on the button prefab!");
        }

        // 버튼이 생성된 후의 추가적인 로직
        buttons.Add(buttonObj);
    }

    // 버튼 클릭시 호출될 메소드
    void ButtonClicked(double time, string text)
    {
        feedbackText.text = "";
        // 여기에 피드백에 대한 처리 로직을 구현합니다.
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

        //해당 초에 맞는 인덱스를 찾아 리플레이 시작
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

        //초기자세로 이동

        Vector3 initV = targetManager.getInitPos();

        x = initV.x;
        y = initV.y;
        z = initV.z;

        targetManager.setInitPos();
        
        //시간별 버튼들 보이기
        buttonsParent.gameObject.SetActive(true);
        //리플레이 요소 보이기
        playButton.gameObject.SetActive(true);
        stopButton.gameObject.SetActive(true);
        replaySlider.gameObject.SetActive(true);
        replayUI.gameObject.SetActive(true);
        endButton.gameObject.SetActive(false);

        if(DataManager.Instance != null)replaySlider.maxValue = DataManager.Instance.getRTTime();

        //그래프 만들기
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

			// MoveObject 코루틴을 호출하여 각 지점으로 천천히 이동
			if (i != 0)
			{
				yield return StartCoroutine(targetManager.MoveObject(startPosition, endPosition, allPointsBarbell[i].Item1 - allPointsBarbell[i - 1].Item1)); // 1.0f는 이동에 걸리는 시간, 필요에 따라 조절하세요.
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
 