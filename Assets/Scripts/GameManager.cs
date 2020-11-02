using UnityEngine;
using System.Collections;

using System.Collections.Generic;        //Allows us to use Lists. 
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f; 

    public static GameManager instance = null;                //Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager boardScript;                        //Store a reference to our BoardManager which will set up the level.

    public int playerStarPoints = 0;
    public int playerSteps = 0;
    public int playerLives = 3;

    public int remainingLevelViews = 3;

    public bool seeWorld = false;

    [HideInInspector] public bool playersTurn = true;


    private int level = 1;                                    //Current level number, expressed in game as "Level 1".
    private Text levelText;

    private int remainingViews;
    public Text viewLevelText;


    private GameObject levelImage;
    private bool doingSetup = true;


    public float timeRemaining = 3;
    public bool timerIsRunning = false;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }


    private void OnLevelWasLoaded (int index)
    {
        level++;

        BoardManager.starPositions = new List<Vector3>();
        BoardManager.enemyPositions = new List<Vector3>();

        InitGame();
    }
    //Initializes the game for each level.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;

        viewLevelText = GameObject.Find("ViewText").GetComponent<Text>();
        viewLevelText.text = remainingLevelViews + "/3";

        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        boardScript.SetupScene(level);

    }
    public void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    void Update()
    {
        //Check that playersTurn or enemiesMoving or doingSetup are not currently true.
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
            }
            else
            {
                seeWorld = false;
                BoardManager.HideElements(seeWorld);
                timeRemaining = 0;
                timerIsRunning = false;
            }
        }
        if (playersTurn  || doingSetup)
            //If any of these are true, return and do not start MoveEnemies.
            return;
    }
    public void GameOver()
    {
        if(level == 1)
            levelText.text = "After " + level + " level, you died.";
        else
            levelText.text = "After " + level + " levels, you died.";
        levelImage.SetActive(true);
        enabled = false; 
    }
}
