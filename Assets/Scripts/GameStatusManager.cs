using GameState;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStatusManager : MonoBehaviour
{
    public string NextScene;
    public Text gameStatusText;
    public static int randomLevel = 1;
    // Start is called before the first frame update
    void Start()
    {
        GlobalGameState.resetScene();
        gameStatusText.gameObject.SetActive(false);
        gameStatusText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (GlobalGameState.isGameOver) 
        {
            gameStatusText.gameObject.SetActive(true);
            if (gameObject.GetComponent<CountDown>().isGameWin())
            {
                ShowGameWin();
                Invoke("LoadNextScene", 2);
            }
            else 
            {
                ShowGameLose();
            }
        }
    }

    private void ShowGameWin() {
        gameStatusText.text = "YOU WIN!";
    }

    private void ShowGameLose() {
        gameStatusText.text = "YOU LOSE!";
    }

    void LoadNextScene()
    {
        if (NextScene != "")
        {
            GameObject.FindGameObjectWithTag("NaviInterface").GetComponent<NavMeshSurface>().RemoveData();
            CountDown.resetScene();
            SceneManager.LoadScene(NextScene);
        }
    }
}
