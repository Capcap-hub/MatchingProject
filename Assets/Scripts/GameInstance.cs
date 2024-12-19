using System;
using System.Collections.Generic;
using UnityEngine;

public class GameInstance: MonoBehaviour
{
    public static GameInstance Instance;
    
    public UserInfo userInfo;
    public ScoreInfo scoreInfo;
    
    private void Awake()
    {
        // 确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
            userInfo = new UserInfo();
            scoreInfo = new ScoreInfo();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[Serializable]
public class ApiResponse<T>
{
    public int code;
    public string msg;
    public T data;
    public bool success;
}

[Serializable]
public class ApiListResponse<T>
{
    public int code;
    public string msg;
    public List<T> data;
    public bool success;
}

[Serializable]
public class UserInfo
{
    public string telegramId;
    public string gameId;
    public string accountId;
}

[Serializable]
public class ScoreInfo
{
    public string gameId;
    public string accountId;
    public string score;
    public string maxScore;
    public string maxScoreTime;
    public string totalScore;
    public string playCount;
}

[Serializable]
public class ToolItemInfo
{
    public string gameId;
    public string accountId;
    public string telegramId;
    public string gameItemId;
    public int num;
}