using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Patonki : MonoBehaviour {
    public SteamVR_Action_Vibration touchFeedback;
    public bool isRightHand;
    Interactable interactable;
    float feedbackAmplitude = 75;
    float feedbackFrequency = 200;
    float feedbackLength  = .15f;

    private void Awake() {
        interactable = GetComponent<Interactable>();    
    }

    private void OnTriggerEnter(Collider other) {
        print("Feedback");
        if(isRightHand)
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
        else
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.LeftHand);

    }

}
