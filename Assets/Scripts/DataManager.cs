using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public string exName;
    private int exLaps;
    private float exWeight;
    private string explainInfo = "";

    private float realtimeTime= 0f;
    private int realtimeLaps =0;
    private bool isReplay = false;

	public void reset()
	{
        exName = "";
        exLaps = 0;
        exWeight = 0f;

        realtimeTime = 0f;
        realtimeLaps = 0;
        isReplay = false;
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Awake()
    {
        if(Instance == null)
		{
            Instance = this;
            DontDestroyOnLoad(gameObject);
		}
		else
		{
            Destroy(gameObject);
		}
    }

    public void setExName(string name)
	{
		exName = name;
	}

    public void setExLaps(int laps)
	{
        exLaps = laps;
	}

    public void setExWeight(float weight)
	{
        exWeight = weight;
	}

    public string getExName()
	{
        return exName;
	}

    public int getExLaps()
    {
        return exLaps;
    }
    public float getExWeight()
    {
        return exWeight;
    }

    public void setRTTime(float time)
    {
        realtimeTime= time;
    }

    public float getRTTime()
    {
        return realtimeTime;
    }


    public void setRTLaps(int lap)
    {
        realtimeLaps = lap;
    }

    public int getRTLaps()
    {
        return realtimeLaps;
    }

    public void setIsReplay(bool val)
	{
        isReplay = val;
	}

    public bool getIsReplay()
	{
        return isReplay;
	}

    public void setExplainInfo(string s)
	{
        explainInfo = s;
	}

    public string getExplainInfo()
	{
        return explainInfo;
	}
}
