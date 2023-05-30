using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

public class ChatGPT : MonoBehaviour
{
    // OpenAI API密钥
    private string apiKey = "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";

    // 要生成的文本长度
    public int textLength = 100;

    // 要生成的文本数量
    public int numTexts = 1;

    // ChatGPT API端点
    private string endpoint = "https://api.openai.com/v1/engines/davinci-codex/completions";

    [LabelText("内容")] 
    public string prompt;
    
    [LabelText("回答")] 
    public string response;
    
    [Button("提交")]
    public void Submit()
    {
        StartCoroutine(GetChatGPTResponse());
    }

    IEnumerator GetChatGPTResponse()
    {
        // 准备API请求数据
        string data = "{\"prompt\": \"IK是啥\", \"max_tokens\": " + textLength + ", \"n\": " + numTexts + ", \"stop\": \"\\n\"}";
        UnityWebRequest request = UnityWebRequest.Post(endpoint, data);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // 发送API请求
        yield return request.SendWebRequest();

        // 处理API响应
        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            this.response = response;
            Debug.Log(response);
        }
        else
        {
            Debug.Log(request.error);
        }
    }
}