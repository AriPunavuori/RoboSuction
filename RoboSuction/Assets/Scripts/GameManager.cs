using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public float health;
    public Vector3 spawnPoint;
    public GameObject player;
    public GameObject enemy;
    public float spawnTimer = .75f;
    public float spawnTime = 5f;

    void Start() {
        EnemySpawner();
    }


    void Update() {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0) {
            EnemySpawner();
            spawnTimer = spawnTime;
        }
    }

    public void EnemySpawner() {
        var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoint,Vector3.up);
        Instantiate(enemy, spawnPoint, Quaternion.LookRotation(f));
    }
}
