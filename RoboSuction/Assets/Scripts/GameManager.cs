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
    float textTime = 4f;
    bool gameEnded;
    bool waitingForKills;
    float spawnTime = 1f;

    private void Start() {
        waveName.Add("Warm Up");
        waveName.Add("Wave 1");
        waveName.Add("Wave 2");
        waveName.Add("Wave 3");
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
                    uiText.text = waveName[waveNumber];
                    textTimer = textTime;
                    waveStarted = true;
                }
            }
        } else {
            uiText.text = waveName[waveName.Count-1];
            SetWaveText();
        }
    }

    public void EnemySpawner() {

        if((int)waveInfo[waveNumber].x >= enemiesSpawned) {
            int spawnpointIndex = Random.Range(0, spawnPoints.Length);
            int enemyIndex = Random.Range((int)waveInfo[waveNumber].z, (int)waveInfo[waveNumber].w + 1);
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
        wave1.text = "Wave:" + (waveNumber + 1);
        wave2.text = "Wave:" + (waveNumber + 1);
    }
}
