using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

public class HttpsClient : MonoBehaviour
{
    public static string baseUrl = "https://xgame.findsdk.com/xgame";
    public static string loginUrl = baseUrl + "/v1/account/login";
    public static string updateScoreUrl = baseUrl + "/v1/game/record/add";
    public static string scoreInfoUrl = baseUrl + "/v1/game/record/list";
    public static string handleToolItemUrl = baseUrl + "/v1/game/item/calc";
    public static string getToolItemUrl = baseUrl + "/v1/game/item/list";
    
    // 单例模式（可选，方便其他类访问）
    public static HttpsClient Instance;

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

    // GET 请求方法
    public void GetRequest(string url, Action<string> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GetRequestCoroutine(url, onSuccess, onError));
    }

    private IEnumerator GetRequestCoroutine(string url, Action<string> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        // 自定义请求头（根据需要）
        request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN_HERE");

        // 发送请求
        yield return request.SendWebRequest();

        // 处理结果
        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(request.error);
        }
    }

    // POST 请求方法
    public void PostRequest(string url, string jsonData, Action<string> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(PostRequestCoroutine(url, jsonData, onSuccess, onError));
    }

    private IEnumerator PostRequestCoroutine(string url, string jsonData, Action<string> onSuccess, Action<string> onError)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 自定义请求头（根据需要）
        request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN_HERE");

        // 发送请求
        yield return request.SendWebRequest();

        // 处理结果
        if (request.result == UnityWebRequest.Result.Success)
        {
            onSuccess?.Invoke(request.downloadHandler.text);
        }
        else
        {
            onError?.Invoke(request.error);
        }
    }
    
    public static T ParseData<T>(string jsonResponse)
    {
        // 1. 解析出整个JSON对象
        JObject response = JObject.Parse(jsonResponse);

        // 2. 提取 data 部分
        JToken data = response["data"];

        if (data != null)
        {
            return data.ToObject<T>();
        }
        else
        {
            Debug.LogError("Failed to extract data from JSON");
            return default;
        }
    }
}
