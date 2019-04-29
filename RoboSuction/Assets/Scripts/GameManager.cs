using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI uiText;
    public Vector3[] spawnPoints;
    public GameObject player;
    public GameObject enemy;
    public Slider healthBar;
    public float spawnTimer = 1f;
    public int playerHealth = 100;
    float spawnTime = 1f;

    void Update() {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0) {
            int index = Random.Range(0, spawnPoints.Length);
            //EnemySpawner(spawnPoints[index]);
            spawnTimer = spawnTime;
        }
    }

    public void EnemySpawner(Vector3 spawnPoint) {
        var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoint, Vector3.up);
        Instantiate(enemy, spawnPoint, Quaternion.LookRotation(f));
    }

    public void SetHealth(int amount) {
        playerHealth += amount;
        var dh = Mathf.Ceil(playerHealth / 20f);
        
        healthBar.value = dh / 5;
        if (playerHealth < 1) {
            uiText.text = ("U R DED!!!!!!!1");
            Time.timeScale = 0;
        }
    }

}
