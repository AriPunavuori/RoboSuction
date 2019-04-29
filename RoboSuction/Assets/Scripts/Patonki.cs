using System.Collections;
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
    Rigidbody rb;
<<<<<<< HEAD

    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
=======
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate() {
>>>>>>> 531dcba4b98022a505dd7a1f521317a5d5919e46
        rb.rotation = attachmentPoint.rotation;
        rb.position = attachmentPoint.position;
    }

    private void OnCollisionEnter(Collision collision) {
        if(interactable.attachedToHand != null) {
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
            print("Feedback");
        }
    }

}
