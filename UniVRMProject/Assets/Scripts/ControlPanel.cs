using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRM;

public class ControlPanel : MonoBehaviour {
    [SerializeField] TMP_InputField MyInput;
    [SerializeField] VRMBlendShapeProxy MyVRMShpae;
    [SerializeField] BlendShapePreset Fun;

    private void Start() {

    }
    public async void ConfirmInput() {
        MyVRMShpae.gameObject.SetActive(false);
        MyVRMShpae.gameObject.SetActive(true);
        MyVRMShpae.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(Fun), 1.0f);
        string prompt = MyInput.text;
        MyInput.text = "Loading";
        //OpenAIManager.Chat2(MyInput.text).ContinueWith(task => {
        //    Debug.Log("b");
        //    MyInput.text = task.Result;
        //});

        MyInput.text = await OpenAIV1Manager.Completion(prompt);
    }
}
