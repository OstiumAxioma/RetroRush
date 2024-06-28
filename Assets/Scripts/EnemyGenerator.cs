using GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    // Reference to the enemy prefab
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyNum = 5;
    [SerializeField] private float interval = 1.0f;
    [SerializeField] private int leftEnemies = 0;

    private float timer = 0.0f; // timer

    public void Start()
    {
        //leftEnemies = 0;
    }

    public void Update()
    {
        //Debug.Log("Spawn ID:" + this.GetInstanceID() + ",leftEnemies:" + leftEnemies + ",CountDown.isCurrentLevelOver:" + CountDown.isCurrentLevelOver);
        if (leftEnemies > 0 && !GlobalGameState.isCurrentLevelOver) {
            timer += Time.deltaTime;
            // Debug.Log("time:" + timer);
            if (timer > interval)
            {
                CreateEnemy();
                leftEnemies--;
                timer = 0.0f;
            }
        }
    }

    public void ActivateGenerator() {
        //sDebug.Log("activated");
        leftEnemies = enemyNum;
        timer = 0.0f;
    }

    // Method to create an enemy at the factory's location
    public void CreateEnemy()
    {
        // Instantiate the enemy prefab at the factory's position and rotation
        GameObject enemy = Instantiate(enemyPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), enemyPrefab.transform.rotation);
    }
}
