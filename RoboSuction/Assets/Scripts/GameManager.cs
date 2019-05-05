using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct LevelData {
    public int numberOfEnemies;
    public float spawnInterval;
    public GameObject[] enemies;
    public Vector3[] spawnpoints;

    public LevelData(int nmbrfnms, float spwnntrvl, GameObject[] nms, Vector3[] spwnpts) {
        numberOfEnemies = nmbrfnms;
        spawnInterval = spwnntrvl;
        enemies = nms;
        spawnpoints = spwnpts;
    }
}

public class GameManager : MonoBehaviour {

    public TextMeshProUGUI uiText;
    public TextMeshProUGUI wave1;
    public TextMeshProUGUI wave2;
    public TextMeshProUGUI kills1;
    public TextMeshProUGUI kills2;

    public Vector3[] spawnPoints;
    public GameObject[] enemies;
    public Vector4[] waveInfo;

    public LevelData[] leveldata;

    public List<string> waveName;

    public int enemiesSpawned;
    public int enemiesKilled = 0;
    public int waveNumber = 0;

    float textTimer;
    float textTime = 10f;
    float spawnTimer = 10f;
    float spawnTime = 1f;
    float waveBreak = 5f;

    public GameObject player;
    public Slider healthBar;

    public int playerHealth = 100;
    public bool waveStarted;

    public bool hasGameEnded;
    //public bool waitingForKills;


    private void Start() {
        waveName.Add("Time for Warm Up!");
        waveName.Add("Here we go!");
        waveName.Add("Ready 4 more?");
        waveName.Add("Just survive!");
        waveName.Add("You Won!");
        waveName.Add("Your life currentcy has been sucked dry!");
        uiText.text = waveName[waveNumber];
        SetWaveText();
        SetKillText();
        textTimer = textTime;
    }

    void Update() {

        if(!hasGameEnded) {
            spawnTimer -= Time.deltaTime;
            textTimer -= Time.deltaTime;

            if(textTimer < 0) {
                uiText.text = "";
            }

            if(EnemiesToSpawn() && spawnTimer < 0) {
                EnemySpawner();
            }

            if(!EnemiesToSpawn() && !MoreEnemiesSpawnedThanKilled()) {
                ResetWave();
            }
        }
    }

    public void ResetWave() {

        if(IsLastRound() && !MoreEnemiesSpawnedThanKilled()) {
            hasGameEnded = true;
        }

        if(!hasGameEnded) {
            if(!IsLastRound())
            waveNumber++;
            enemiesSpawned = 0;
            enemiesKilled = 0;
            SetUIText(waveName[waveNumber]);
            SetKillText();
            SetWaveText();
            textTimer = textTime;
            spawnTimer = waveBreak;
        } else {
            SetUIText(waveName[waveName.Count-2]);
        }
    }

    public bool EnemiesToSpawn() {
        return (int)waveInfo[waveNumber].x > enemiesSpawned && !hasGameEnded;
    }

    public bool IsLastRound() {
        return waveNumber >= waveInfo.Length - 1;
    }

    public bool MoreEnemiesSpawnedThanKilled() {
        return enemiesKilled < enemiesSpawned;
    }

    public void EnemySpawner() {
        int spawnpointIndex = Mathf.Clamp(Random.Range(0, waveNumber * 2 + 2), 0, spawnPoints.Length);
        int enemyIndex = Random.Range((int)waveInfo[waveNumber].z, (int)waveInfo[waveNumber].w + 1);
        var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoints[spawnpointIndex], Vector3.up);

        Instantiate(enemies[enemyIndex], spawnPoints[spawnpointIndex], Quaternion.LookRotation(f));

        spawnTimer = waveInfo[waveNumber].y;
        enemiesSpawned++;
    }

    public void SetHealth(int amount) {

        playerHealth += amount;
        if(playerHealth > 100)
            playerHealth = 100;
        var dh = Mathf.Ceil(playerHealth / 20f);

        healthBar.value = dh / 5;
        if(playerHealth < 1) {
            uiText.text = waveName[waveName.Count - 1];
            textTimer = textTime;
            Time.timeScale = 0;
        }
    }

    public void SetUIText(string text) {
        uiText.text = text;
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
