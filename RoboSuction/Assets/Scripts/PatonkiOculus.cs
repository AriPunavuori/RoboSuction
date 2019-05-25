using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PatonkiOculus : MonoBehaviour {
    public GameObject powerOnEffects;
    public GameObject spark;
    //public SteamVR_Action_Vibration touchFeedback;
    public AudioClip pickUp;
    public AudioSource patonki;
    GameManager gm;
    OVRGrabbable grabbable;
    //float feedbackAmplitude = 75;
    //float feedbackFrequency = 200;
    //float feedbackLength = .15f;

    public bool isRightHand;

    int enemyLayer;

    private void Awake() {
        grabbable = GetComponent<OVRGrabbable>();
        enemyLayer = LayerMask.NameToLayer("Enemy");
        gm = FindObjectOfType<GameManager>();
    }

    void OnCollisionEnter(Collision collision) {
        FeedBack();
        var point = collision.GetContact(0);
        var go = collision.gameObject;
        var sparky = Instantiate(spark, point.point, Quaternion.identity);
        sparky.transform.SetParent(go.transform);
        sparky.GetComponent<ParticleSystem>().Play();
        Destroy(sparky, 3f);
        if(go.layer == enemyLayer) {
            var ec = go.GetComponentInParent<EnemyController>();
            ec.BotHit(collision.collider.name);
        }
    }

    public void FeedBack() {
        //if(isRightHand)
        //    touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.RightHand);
        //else
            //touchFeedback.Execute(0, feedbackLength, feedbackFrequency, feedbackAmplitude, SteamVR_Input_Sources.LeftHand);
    }

    //private void OnEnable() {
    //    grabbable.OnGrabBegin.AddListener(OnGrabbed);
    //    grabbable.OnGrabEnd.AddListener(OnReleased);
    //}

    //private void OnDisable() {
    //    grabbable.OnGrabBegin.RemoveListener(OnGrabbed);
    //    grabbable.OnGrabEnd.RemoveListener(OnReleased);
    //}

    //public void OnGrabbed() {
    //    PowerOn();
    //}

    //public void OnReleased() {
    //    PowerOff();
    //}

    public void PowerOn() {
        powerOnEffects.gameObject.SetActive(true);
        patonki.PlayOneShot(pickUp, 0.5f);
        gm.CountSticksInHand(1);
        FeedBack();
    }

    public void PowerOff() {
        powerOnEffects.gameObject.SetActive(false);
        gm.CountSticksInHand(-1);
    }
}


