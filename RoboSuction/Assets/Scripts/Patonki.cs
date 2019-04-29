using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Patonki : MonoBehaviour {
    public Transform attachmentPoint;
    public SteamVR_Action_Vibration touchFeedback;
    Interactable interactable;
    public float feedbackAmplitude = 100;
    public float feedbackFrequency = 100;
    public float feedbackLength  = 1;

    //private void OnCollisionEnter(Collision collision) {
    //    if(interactable.attachedToHand != null) {
    //        touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
    //        print("Feedback");
    //    }
    //}
    private void Awake()
    {
        interactable = GetComponent<Interactable>();    
    }

    private void OnTriggerEnter(Collider other)
    {
       
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
            print("Feedback");
        
    }

}
