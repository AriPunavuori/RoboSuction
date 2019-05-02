using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Patonki : MonoBehaviour {
    public SteamVR_Action_Vibration touchFeedback;
    public bool isRightHand;
    public GameObject powerOnEffects;
    public GameObject spark;
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

    void OnCollisionEnter(Collision collision) {
        var point = collision.GetContact(0);
        spark = Instantiate(spark, point.point, Quaternion.identity);
        spark.GetComponent<ParticleSystem>().Play();
    }

    public void PowerOn() {
        powerOnEffects.gameObject.SetActive(true);
    }

    public void PowerOff() {
        powerOnEffects.gameObject.SetActive(false);
    }
}

