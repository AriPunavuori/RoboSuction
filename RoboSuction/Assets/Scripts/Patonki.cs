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

    void OnCollisionEnter(Collision collision) {
        FeedBack();
        var point = collision.GetContact(0);
        var go = collision.gameObject;
        var sparky = Instantiate(spark, point.point, Quaternion.identity);
        sparky.transform.SetParent(go.transform);
        sparky.GetComponent<ParticleSystem>().Play();
        Destroy(spark, 3f);
    }

    public void FeedBack() {
        if (isRightHand)
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
        else
            touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.LeftHand);
    }

    public void PowerOn() {
        powerOnEffects.gameObject.SetActive(true);
        FeedBack();
    }

    public void PowerOff() {
        powerOnEffects.gameObject.SetActive(false);
    }
}

