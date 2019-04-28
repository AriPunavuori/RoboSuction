using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Vector3 spawnPoint;
    public GameObject player;
    public GameObject enemy;
    public Slider healthBar;
    public float spawnTimer = 1f;
    public int playerHealth = 100;
    float spawnTime = 1f;

    void Update() {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0) {
            //EnemySpawner();
            spawnTimer = spawnTime;
        }
    }

    public void EnemySpawner() {
        var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoint,Vector3.up);
        Instantiate(enemy, spawnPoint, Quaternion.LookRotation(f));
    }

    public void SetHealth(int amount) {
        playerHealth -= amount;
        healthBar.value = playerHealth / 100f;
    }

}
