using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using System.Threading.Tasks;

public class AzureTTSManager : MonoBehaviour {
    [SerializeField] AudioSource MyAudioSource;
    string apiKey = "070ff04696714320a2ebd02bc76a77af";
    string toeknEndpoint = "https://eastus.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
    string ttsEndpoint = "https://eastus.tts.speech.microsoft.com/cognitiveservices/v1";
    string accessToken = "";

    static AzureTTSManager Instance;

    void Start() {
        Instance = this;
    }

    public static void Talk(string _text) {
        Debug.Log(_text);
        if (string.IsNullOrEmpty(_text)) return;
        Instance.StartCoroutine(Instance.GetTokenAndSynthesizeText(_text));
    }

    IEnumerator GetTokenAndSynthesizeText(string text) {
        //Debug.Log("toeknEndpoint=" + toeknEndpoint);
        //Debug.Log("apiKey=" + apiKey);
        using (UnityWebRequest tokenRequest = new UnityWebRequest(toeknEndpoint, "POST")) {
            tokenRequest.SetRequestHeader("Ocp-Apim-Subscription-Key", apiKey);
            tokenRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return tokenRequest.SendWebRequest();

            if (tokenRequest.result == UnityWebRequest.Result.Success) {
                accessToken = tokenRequest.downloadHandler.text;

                StartCoroutine(SynthesizeText(text, accessToken));
            } else {
                Debug.LogError("Failed to fetch token: " + tokenRequest.error);
            }
        }
    }

    IEnumerator SynthesizeText(string text, string _token) {
        //Debug.Log("text=" + text);
        //Debug.Log("token=" + _token);
        //Debug.Log("ttsEndpoint=" + ttsEndpoint);
        using (UnityWebRequest www = new UnityWebRequest(ttsEndpoint, "POST")) {
            // Set the request headers
            www.SetRequestHeader("Content-Type", "application/ssml+xml");
            www.SetRequestHeader("Authorization", "Bearer " + _token);
            www.SetRequestHeader("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");
            www.SetRequestHeader("User-Agent", "TestProject ");

            // Create the SSML payload
            string ssml = $"<speak version='1.0' xml:lang='en-US'><voice xml:lang='en-US' xml:gender='Female' style='whispering' styledegree='1' role='Girl' name = 'en-US-JennyNeural'>{ text}</voice></speak>";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(ssml);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);

            // Set up download handler to save the response as an audio file
            //www.downloadHandler = new DownloadHandlerFile(Path.Combine(Application.dataPath, "output.wav"));
            // Set up download handler to load the response as an AudioClip
            www.downloadHandler = new DownloadHandlerAudioClip(www.url, AudioType.WAV);
            // Send the request and wait for the response
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success) {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                MyAudioSource.clip = audioClip;
                MyAudioSource.Play();
                Debug.Log("Text to speech synthesis succeeded!");
            } else {
                Debug.LogError("Text to speech synthesis failed: " + www.error);
            }
        }
    }
}