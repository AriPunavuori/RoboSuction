using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMover : MonoBehaviour
{
    GameManager gm;
    float waitTime = 3f;
    float speed = 2f;
    void Start(){
        gm = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        if (gm.gameEnded && gm.enemiesSpawned == gm.enemiesKilled) {
            waitTime -= Time.deltaTime;
            if (waitTime < 0)
            {
                transform.Translate(Vector3.up * Time.deltaTime * speed, Space.World);
            }
        }
    }
}