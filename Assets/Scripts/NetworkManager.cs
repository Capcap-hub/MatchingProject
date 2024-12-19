using System.Collections.Generic;
using UnityEngine;

public class NetworkManager: MonoBehaviour
{
    public static NetworkManager Instance;
    
    private void Awake()
    {
        // 确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // string fullUrl = Application.absoluteURL;
        string fullUrl = "https://capcap-hub.github.io/MatchingProject/?user_id=7200931463&username=moduoli&full_name=Moduo+Li";
        
        // // 检查 URL 是否为空
        if (string.IsNullOrEmpty(fullUrl))
        {
            Debug.LogWarning("URL 为空或未定义。请确保通过 WebGL 部署运行！");
            return; // 停止进一步解析
        }
        Debug.Log("完整 URL: " + fullUrl);

        // 使用 URLParser 解析查询参数
        Dictionary<string, string> queryParams = URLParser.ParseQueryString(fullUrl);

        if (queryParams.ContainsKey("user_id"))
        {
            string userId = queryParams["user_id"];
            Debug.Log("用户ID: " + userId);
            GameInstance.Instance.userInfo.telegramId = userId;
        }

        if (queryParams.ContainsKey("username"))
        {
            string username = queryParams["username"];
            Debug.Log("用户名: " + username);
            // userInfo += $"UserName: {username}\n";
        }

        if (queryParams.ContainsKey("full_name"))
        {
            string fullName = queryParams["full_name"];
            Debug.Log("用户全名: " + fullName);
            // userInfo += $"FullName: {fullName}\n";
        }

        GameInstance.Instance.userInfo.gameId = "CapCapMatching";
        GameInstance.Instance.scoreInfo.gameId = GameInstance.Instance.userInfo.gameId;
        
        //登录
        string jsonData = JsonUtility.ToJson(GameInstance.Instance.userInfo); 
        HttpsClient.Instance.PostRequest(HttpsClient.loginUrl,jsonData,onSuccess: response =>
            {
                Debug.Log("POST Success: " + response);
                UserInfo userInfo = HttpsClient.ParseData<UserInfo>(response);
                if (userInfo != null)
                {
                    GameInstance.Instance.userInfo.accountId = userInfo.accountId;
                    GameInstance.Instance.scoreInfo.accountId = userInfo.accountId;
                    GetScoreInfo();
                    GetToolItem();
                }
            },
            onError: error =>
            {
                Debug.LogError("POST Error: " + error);
            });
        
        
    }

    public void updateScore(int score)
    {
        ScoreInfo scoreInfo = new ScoreInfo()
        {
            gameId = GameInstance.Instance.userInfo.gameId,
            accountId = GameInstance.Instance.userInfo.accountId,
            score = score.ToString()
        };
        
        string jsonData = JsonUtility.ToJson(scoreInfo); 
        HttpsClient.Instance.PostRequest(HttpsClient.updateScoreUrl, jsonData,onSuccess: response =>
            {
                Debug.Log("POST Success: " + response);
            },
            onError: error =>
            {
                Debug.LogError("POST Error: " + error);
            }); 
    }

    public void GetScoreInfo()
    {
        string jsonData = JsonUtility.ToJson(GameInstance.Instance.scoreInfo); 
        HttpsClient.Instance.PostRequest(HttpsClient.scoreInfoUrl, jsonData, onSuccess: response =>
            {
                Debug.Log("POST Success: " + response);
                ScoreInfo scoreInfo = HttpsClient.ParseData<ScoreInfo>(response);
                GameInstance.Instance.scoreInfo = scoreInfo;
                Debug.Log(scoreInfo);
            },
            onError: error =>
            {
                Debug.LogError("POST Error: " + error);
            }); 
    }

    public void AddToolItem(int itemId, int addCount)
    {
        ToolItemInfo toolItemInfo = new ToolItemInfo()
        {
            gameId = GameInstance.Instance.userInfo.gameId,
            accountId = GameInstance.Instance.userInfo.accountId,
            gameItemId = itemId.ToString(),
            num = addCount
        };
        string jsonData = JsonUtility.ToJson(toolItemInfo); 
        HttpsClient.Instance.PostRequest(HttpsClient.handleToolItemUrl, jsonData,onSuccess: response =>
            {
                Debug.Log("POST Success: " + response);
            },
            onError: error =>
            {
                Debug.LogError("POST Error: " + error);
            }); 
    }

    public void GetToolItem()
    {
        ToolItemInfo toolItemInfo = new ToolItemInfo()
        {
            gameId = GameInstance.Instance.userInfo.gameId,
            accountId = GameInstance.Instance.userInfo.accountId,
            telegramId = GameInstance.Instance.userInfo.telegramId,
        };
        string jsonData = JsonUtility.ToJson(toolItemInfo); 
        HttpsClient.Instance.PostRequest(HttpsClient.getToolItemUrl, jsonData,onSuccess: response =>
            {
                Debug.Log("POST Success: " + response);
                List<ToolItemInfo> toolItemInfos = HttpsClient.ParseData<List<ToolItemInfo>>(response);
                foreach (ToolItemInfo toolItemInfo in toolItemInfos)
                {
                    EventManager.Instance.Broadcast(GameEvents.UpdateToolItemCountEvent, int.Parse(toolItemInfo.gameItemId), toolItemInfo.num);
                }
            },
            onError: error =>
            {
                Debug.LogError("POST Error: " + error);
            }); 
    }
}