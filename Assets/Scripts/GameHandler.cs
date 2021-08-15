using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    private static GameHandler instance;
    private static int score;
    [SerializeField] private Snake snake;
    private LevelGrid levelGrid;
    private void Awake()
    {
        instance = this;
       // InitializeStatic();
        buttonRestart.onClick.(ReloadLevel);
    }
    private void Start()
    {
        Debug.Log("GameHandler.start");
        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
/*        ButtonUI(Vector2.zero, "Reload Level", () =>
        {
            Loader.Load(Loader.Scene.MainScene);
        });*/
    }
    public Button buttonRestart;
    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
   /* private static void InitializeStatic()
    {
        score = 0;
    }*/
    public static int GetScore()
    {
        return score;
    }
    public static void AddScore()
    {
        score += 100;
    }
 }
