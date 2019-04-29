﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Patonki : MonoBehaviour {
    public Transform attachmentPoint;
    public SteamVR_Action_Vibration touchFeedback;
    Interactable interactable;
    public float feedbackAmplitude;
    public float feedbackFrequency;
    public float feedbackLength;

    private void OnCollisionEnter(Collision collision) {
        if(interactable.attachedToHand != null) {
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
            print("Feedback");
        }
    }

}