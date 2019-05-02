using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMover : MonoBehaviour
{
    GameManager gm;
    void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (gm.gameEnded) {
            transform.position += transform.position + Vector3.up * Time.deltaTime;
        }
    }
}