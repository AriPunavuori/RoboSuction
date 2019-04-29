using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI uiText;
    public Vector3[] spawnPoints;
    public GameObject[] enemies;
    public Vector4[] waveInfo;
    public int enemiesSpawned;
    public int enemiesKilled = 0;
    public List<string> waveName;
    int waveNumber = 0;
    public GameObject player;
    public Slider healthBar;
    float spawnTimer = 10f;
    public int playerHealth = 100;
    public bool waveStarted;
    float textTimer;
    float textTime = 4f;
    bool gameEnded;
    bool waitingForKills;
    float spawnTime = 1f;

    private void Start() {
        waveName.Add("Warm Up");
        waveName.Add("Wave 1");
        waveName.Add("Wave 2");
        waveName.Add("Wave 3");
        uiText.text = waveName[waveNumber];
        textTimer = textTime;
    }

    void Update() {
        textTimer -= Time.deltaTime;
        if(textTimer < 0) {
            uiText.text = "";
        }

        if(!gameEnded) {
            if(!waitingForKills) {
                spawnTimer -= Time.deltaTime;
                if(spawnTimer < 0) {
                    EnemySpawner();
                }
            } else {
                if(enemiesKilled >= enemiesSpawned) {
                    waitingForKills = false;
                    enemiesSpawned = 0;
                    enemiesKilled = 0;
                    uiText.text = waveName[waveNumber];
                    textTimer = textTime;
                    waveStarted = true;
                }
            }
        }
    }

    public void EnemySpawner() {

        if((int)waveInfo[waveNumber].x >= enemiesSpawned) {
            int spawnpointIndex = Random.Range(0, spawnPoints.Length);
            int enemyIndex = Random.Range((int)waveInfo[waveNumber].z, (int)waveInfo[waveNumber].w);
            print("EnemyIndex:" + enemyIndex);
            spawnTimer = waveInfo[waveNumber].y;

            var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoints[spawnpointIndex], Vector3.up);
            Instantiate(enemies[enemyIndex], spawnPoints[spawnpointIndex], Quaternion.LookRotation(f));
            enemiesSpawned++;
            if(enemiesSpawned >= (int)waveInfo[waveNumber].x) {
                waveNumber++;
                waitingForKills = true;
                print(waitingForKills);
                spawnTimer = 5f;
                if(waveNumber >= waveInfo.Length) {
                    gameEnded = true;
                }
            }
        }
    }

    public void SetHealth(int amount) {
        playerHealth += amount;
        var dh = Mathf.Ceil(playerHealth / 20f);

        healthBar.value = dh / 5;
        if(playerHealth < 1) {
            uiText.text = ("U R DED!!!!!!!1");
            Time.timeScale = 0;
        }
    }

}
