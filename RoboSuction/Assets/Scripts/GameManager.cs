using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI uiText;
    public TextMeshProUGUI wave1;
    public TextMeshProUGUI wave2;
    public TextMeshProUGUI kills1;
    public TextMeshProUGUI kills2;
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
    float textTime = 10f;
    bool gameEnded;
    bool waitingForKills;
    float spawnTime = 1f;

    private void Start() {
        waveName.Add("Time for Warm Up!");
        waveName.Add("Here we go!");
        waveName.Add("Ready 4 more?");
        waveName.Add("Just survive!");
        waveName.Add("Your life currentcy has been sucked dry!");
        waveName.Add("You Win!");
        uiText.text = waveName[waveNumber];
        SetWaveText();
        SetKillText();
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
                    SetKillText();
                    uiText.text = waveName[waveNumber];
                    SetWaveText();
                    textTimer = textTime;
                    waveStarted = true;
                }
            }
        } else {
            if (enemiesKilled >= enemiesSpawned) {
                textTimer = textTime;
                uiText.text = waveName[waveName.Count - 1];
            }
        }
    }

    public void EnemySpawner() {

        if((int)waveInfo[waveNumber].x >= enemiesSpawned) {
            int spawnpointIndex = Mathf.Clamp(Random.Range(0, waveNumber * 2 + 2),0,spawnPoints.Length);
            int enemyIndex = Random.Range((int)waveInfo[waveNumber].z, (int)waveInfo[waveNumber].w + 1);
            spawnTimer = waveInfo[waveNumber].y;

            var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoints[spawnpointIndex], Vector3.up);
            Instantiate(enemies[enemyIndex], spawnPoints[spawnpointIndex], Quaternion.LookRotation(f));
            enemiesSpawned++;
            if(enemiesSpawned >= (int)waveInfo[waveNumber].x) {
                waveNumber++;
                waitingForKills = true;
                spawnTimer = 7.5f;
                if(waveNumber >= waveInfo.Length) {
                    gameEnded = true;
                }
            }
        }
    }

    public void SetHealth(int amount) {
        
        playerHealth += amount;
        if (playerHealth > 100)
            playerHealth = 100;
        var dh = Mathf.Ceil(playerHealth / 20f);

        healthBar.value = dh / 5;
        if(playerHealth < 1) {
            print("kuolema");
            uiText.text = waveName[waveName.Count - 2];
            textTimer = textTime;
            Time.timeScale = 0;
        }
    }

    public void SetKillText() {
        kills1.text = "Kills:" + enemiesKilled;
        kills2.text = "Kills:" + enemiesKilled;
    }
    public void SetWaveText() {
        wave1.text = "Wave:" + waveNumber;
        wave2.text = "Wave:" + waveNumber;
    }
}
