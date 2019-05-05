using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMover : MonoBehaviour {

    GameManager gm;
    float waitTime = 3f;
    float speed = 2f;

    void Start() {
        gm = FindObjectOfType<GameManager>();
    }

    void Update() {
        if(gm.hasGameEnded) {
            waitTime -= Time.unscaledDeltaTime;
            if(waitTime < 0) {
                transform.Translate(Vector3.up * Time.unscaledDeltaTime * speed, Space.World);
            }
        }
    }
}