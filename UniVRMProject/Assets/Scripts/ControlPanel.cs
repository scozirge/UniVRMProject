using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRM;

public class ControlPanel : MonoBehaviour {
    [SerializeField] TMP_InputField MyInput;
    [SerializeField] InputField MyInput2;
    [SerializeField] VRMBlendShapeProxy MyVRMShpae;
    [SerializeField] BlendShapePreset Fun;

    private void Start() {
        MyVRMShpae.gameObject.SetActive(false);
        MyVRMShpae.gameObject.SetActive(true);
        MyVRMShpae.ImmediatelySetValue(BlendShapeKey.CreateFromPreset(Fun), 1.0f);
    }
    private void Update() {
        if (Input.GetKey(KeyCode.KeypadEnter)) {
            ConfirmInput();
        }
    }
    public async void ConfirmInput() {

        string prompt = MyInput2.text;
        MyInput2.text = "Loading";
        //OpenAIManager.Chat2(MyInput.text).ContinueWith(task => {
        //    Debug.Log("b");
        //    MyInput.text = task.Result;
        //});

        //MyInput.text = await OpenAIV1Manager.Completion(prompt);
        MyInput2.text = await OpenAIV1Manager.Completion(prompt);
    }
}
