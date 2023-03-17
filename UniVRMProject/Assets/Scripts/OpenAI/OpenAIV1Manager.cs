using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using OpenAI;

public class OpenAIV1Manager : MonoBehaviour {
    [SerializeField] string APIKey = "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";
    [SerializeField] string OrgID = "org-5zCPmSUFTxPSHRzFoGQX4P5g";
    [SerializeField] string Model = "text-davinci-003";
    [SerializeField] int MaxTokens = 300;
    [SerializeField] List<string> SysMemory = new List<string>();
    static OpenAIApi API;

    static List<MyPrompt> AIUserPrompts = new List<MyPrompt>();
    static List<string> Prompts = new List<string>();
    static OpenAIV1Manager Instance;

    private void Start() {
        Instance = this;
        API = new OpenAIApi(APIKey);
    }
    static string GenerateNewMsg(string _prompt) {
        string s = "[";

        MyPrompt thisMsg = new MyPrompt("user", _prompt);
        AIUserPrompts.Add(thisMsg);
        for (int i = 0; i < Instance.SysMemory.Count; i++) {
            MyPrompt message = new MyPrompt("system", Instance.SysMemory[i]);
            s += JsonUtility.ToJson(message);
            s += ",";
        }
        for (int i = 0; i < AIUserPrompts.Count; i++) {
            s += JsonUtility.ToJson(AIUserPrompts[i]);
            if (i != AIUserPrompts.Count - 1)
                s += ",";
        }
        s += "]";
        Debug.Log(s);
        return s;
    }
    public static async Task<string> Chat(string _prompt) {

        var request = new CreateChatCompletionRequest();
        request.Messages = new List<ChatMessage>();
        ChatMessage message = new ChatMessage();
        message.Role = "system";
        message.Content = _prompt;
        request.Messages.Add(message);
        request.Temperature = 1;
        request.MaxTokens = Instance.MaxTokens;
        request.Model = Instance.Model;
        var response = await API.CreateChatCompletion(request);
        //foreach (var choice in response.Choices) {
        //    Debug.Log(choice.Message.Content);
        //}
        AzureTTSManager.Talk(response.Choices[0].Message.Content);
        return response.Choices[0].Message.Content;
    }
    public static async Task<string> Completion(string _prompt) {
        Debug.Log(_prompt);
        Prompts.Add("\nme: " + _prompt);
        string s = "";
        for (int i = 0; i < Prompts.Count; i++) {
            s += Prompts[i];
            s += "\n";
        }
        s += "\n";
        s += "you: ";
        var request = new CreateCompletionRequest();
        request.Model = Instance.Model;
        request.Prompt = s;
        request.Temperature = 1;
        request.MaxTokens = Instance.MaxTokens;
        request.MaxTokens = Instance.MaxTokens;
        request.Model = Instance.Model;
        var response = await API.CreateCompletion(request);
        Prompts.Add("\nyou: " + response.Choices[0].Text);
        Debug.LogError(response.Choices[0].Text);
        AzureTTSManager.Talk(response.Choices[0].Text);
        return response.Choices[0].Text;
    }
}
[Serializable]
struct MyPrompt {
    public string role;
    public string content;
    public MyPrompt(string _role, string _content) {
        role = _role;
        content = _content;
    }
}