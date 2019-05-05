﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct LevelData {
    public int numberOfEnemies;
    public float spawnInterval;
    public GameObject[] enemies;
    public GameObject[] spawnpoints;

    public LevelData(int nmbrfnms, float spwnntrvl, GameObject[] nms, GameObject[] spwnpts) {
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

    public LevelData[] leveldata;

    public List<string> infoTexts;

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
    public bool hasGameEnded;

    private void Start() {
        uiText.text = infoTexts[waveNumber];
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
            SetUIText(infoTexts[waveNumber]);
            SetKillText();
            SetWaveText();
            textTimer = textTime;
            spawnTimer = waveBreak;
        } else {
            SetUIText(infoTexts[infoTexts.Count-2]);
        }
    }

    public bool EnemiesToSpawn() {
        return leveldata[waveNumber].numberOfEnemies > enemiesSpawned && !hasGameEnded;
    }

    public bool IsLastRound() {
        return waveNumber >= leveldata.Length - 1;
    }

    public bool MoreEnemiesSpawnedThanKilled() {
        return enemiesKilled < enemiesSpawned;
    }

    public void EnemySpawner() {
        var spawnpointIndex = Random.Range(0,leveldata[waveNumber].spawnpoints.Length);
        var spawnPoint = leveldata[waveNumber].spawnpoints[spawnpointIndex].transform.position;
        var enemyIndex = Random.Range(0,leveldata[waveNumber].enemies.Length);
        var enemy = leveldata[waveNumber].enemies[enemyIndex];
        var f = Vector3.ProjectOnPlane(player.transform.position - spawnPoint, Vector3.up);
        Instantiate(enemy, spawnPoint, Quaternion.LookRotation(f));

        spawnTimer = leveldata[waveNumber].spawnInterval;
        enemiesSpawned++;
    }

    public void SetHealth(int amount) {

        playerHealth += amount;
        if(playerHealth > 100)
            playerHealth = 100;
        var dh = Mathf.Ceil(playerHealth / 20f);

        healthBar.value = dh / 5;
        if(playerHealth < 1) {
            uiText.text = infoTexts[infoTexts.Count - 1];
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
