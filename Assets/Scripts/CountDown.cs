using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GameState;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace GameState {
    public class GlobalGameState 
    {
        public static bool isCurrentLevelOver = false;
        public static bool isGameOver = false;

        public static void resetScene()
        {
            GlobalGameState.isGameOver = false;
            GlobalGameState.isCurrentLevelOver = false;
        }
    }
}

public class CountDown : MonoBehaviour
{
    public float levelDuration = 20.0f;
    public Text timerText;
    public Text promptText;
    public Text scoreText;
    public PlacementSystem placementSystem;
    public static float countDown;
    private int currentLevel; 
    public int MaxLevel = 5;

    private int totalScore;
    public static int lastScoreInMan = 0;
    public static int lastScoreInBasedBot = 0;

    public AudioSource backgroundSource;
    // Start is called before the first frame update
    void Start()
    {
        currentLevel = 1;
        GlobalGameState.isCurrentLevelOver = false;
        countDown = 5.0f;
        totalScore = 0;
        backgroundSource.Play();
        SetTimerText();
        if (GameObject.FindGameObjectWithTag("NaviInterface") != null) {
            GameObject.FindGameObjectWithTag("NaviInterface").GetComponent<NavMeshSurface>().RemoveData();
            GameObject.FindGameObjectWithTag("NaviInterface").GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        // SetPromptText();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GlobalGameState.isCurrentLevelOver && !GlobalGameState.isGameOver)
        {
            if (GetComponent<ObjectManager>().IsLowerThanGround() || GetComponent<ObjectManager>().IsHealthLowerThanZero()) 
            {
                GlobalGameState.isGameOver = true;
                GetComponent<ObjectManager>().StopMovement();
            }
            if (countDown > 0)
            {
                countDown -= Time.deltaTime;
            }
            else
            {
                countDown = 0.0f;
                GameObject.FindAnyObjectByType<CameraMove>().CallProjectionToInterval();
                CallNextLevel();
            }
            SetTimerText();
            checkScore();
        }
    }

    void InactivePromptText() {
        promptText.gameObject.SetActive(false);
        //promptText.text = "";
    }
    void SetTimerText()
    {
        timerText.text = countDown.ToString("f2");
    }

    void SetScoreText()
    {
        scoreText.text = "Score: " + totalScore.ToString();
    }

    private void checkScore() {
        if (MoreMountains.TopDownEngine.Health.TotalScore > lastScoreInMan) {
            int diff = MoreMountains.TopDownEngine.Health.TotalScore - lastScoreInMan;
            lastScoreInMan = MoreMountains.TopDownEngine.Health.TotalScore;
            totalScore += diff;
        }
        if (EnemyAIFSM.TotalScoreAmount > lastScoreInBasedBot)
        {
            int diff = EnemyAIFSM.TotalScoreAmount - lastScoreInBasedBot;
            lastScoreInBasedBot = EnemyAIFSM.TotalScoreAmount;
            totalScore += diff;
        }
        SetScoreText();
    }

    public bool isGameWin() {
        if (currentLevel < MaxLevel) 
        {
            return false;
        }
        return true;
    }

    void CallNextLevel()
    {
        if (currentLevel == 1) {
            InactivePromptText();
        }
        currentLevel++;
        GlobalGameState.isCurrentLevelOver = true;
        GetComponent<ObjectManager>().CleanGround();
        GetComponent<ObjectManager>().StopMovement();
        if (isGameWin())
        {
            GlobalGameState.isGameOver = true;
            return;
        }
        placementSystem.OnPlacementCompleted += LevelUp;
        GameStatusManager.randomLevel = UnityEngine.Random.Range(1, 3);
        placementSystem.StartPlacement(GameStatusManager.randomLevel);
    }


    public void LevelUp()
    {
        GlobalGameState.isCurrentLevelOver = false;
        GameObject.FindAnyObjectByType<CameraMove>().CallProjectionToStart();
        GetComponent<ObjectManager>().StartMovement();
        //GameObject.FindGameObjectWithTag("NaviInterface").GetComponent<NavMeshSurface>().RemoveData();
        //GameObject.FindGameObjectWithTag("NaviInterface").GetComponent<NavMeshSurface>().BuildNavMesh();
        StartActivateEnemyGenerator();
        countDown = levelDuration;
    }

    private void StartActivateEnemyGenerator() {
        if (currentLevel <= 2) {
            foreach (GameObject enemyGen in GameObject.FindGameObjectsWithTag("EnemyGenerators")) {
                if (enemyGen.layer == 23) {
                    continue;
                }
                enemyGen.GetComponent<EnemyGenerator>().ActivateGenerator();
            }
            return;
        }
        if (SelectEnemyGenerator() == null) {
            return;
        }
        foreach (GameObject enemyGen in SelectEnemyGenerator()) {
            enemyGen.GetComponent<EnemyGenerator>().ActivateGenerator();
        }
    }

    private GameObject[] SelectEnemyGenerator() {
        GameObject[] allGenerators = GameObject.FindGameObjectsWithTag("EnemyGenerators");

        List<GameObject> filteredList = new List<GameObject>();
        foreach (GameObject go in allGenerators)
        {
            if (go.layer != 23)
            {
                filteredList.Add(go);
            }
        }
        if (filteredList.Count == 0)
        {
            return null;
        }
        GameObject[] returnList = filteredList.ToArray();

        int x = returnList.Length;
        System.Random random = new System.Random();
        for (int i = x - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            // exchange position
            GameObject temp = returnList[i];
            returnList[i] = returnList[j];
            returnList[j] = temp;
        }
        int newSize = x - 2 > 0 ? x - 2 : 1;
        GameObject[] result = new GameObject[newSize];
        for (int i = 0; i < newSize; i++)
        {
            result[i] = returnList[i];
        }
        return result;
    }

    public static void resetScene() {
        GlobalGameState.isGameOver = false;
        GlobalGameState.isCurrentLevelOver = false;
    }
}

