using UnityEngine;
using System.Collections;

using System.Collections.Generic;        //Allows us to use Lists. 
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f; 

    public static GameManager instance = null;                //Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManager boardScript;                        //Store a reference to our BoardManager which will set up the level.

    public int playerLives = 1;

    public int remainingLevelViews = 3;


    [HideInInspector] public bool playersTurn = true;

    public int levels = 0;

    private int level = 0;                                    //Current level number, expressed in game as "Level 1".

    private Text levelText;

    public Text viewLevelText;

    public GameObject viewSeeWorldButton;

    public bool skipIntro = false;

    private GameObject levelImage;
    private GameObject godImage;
    private GameObject wonImage;
    private GameObject storyImage;
    private GameObject skipIntroButton;
    private bool doingSetup = true;


    public float timeRemaining = 3;
    public bool timerIsRunning = false;
    public bool seeWorld = false;


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

        //InitGame(); 


    }

    private void OnLevelWasLoaded (int index)
    {
        // if(index == 1)

        if (index == 0)
        {
            level = 0;
            SoundManager.instance.music1Source.Play();

        }
        // if(index == 0)

        if (index == 1)
        {
            level++;
            levels = level;
           
            BoardManager.starPositions = new List<Vector3>();
            BoardManager.enemyPositions = new List<Vector3>();

            InitGame();
        }

    }

    public bool showLevelProgressText = false;
    public bool startGame = false;
    //Initializes the game for each level.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        doingSetup = true;
        skipIntro = false;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();


        levelImage.SetActive(false);


        godImage = GameObject.Find("God");
        godImage.SetActive(false);

        wonImage = GameObject.Find("YouWon");
        wonImage.SetActive(false);

        levelText.text = "Level " + level;        

        viewSeeWorldButton = GameObject.Find("SeeWorldButton");
        viewSeeWorldButton.GetComponent<Button>().gameObject.SetActive(false);
        Debug.Log("level????????" + level);
        if(level == 1 && skipIntro == false)
        {
            Debug.Log("it sure isnt" + skipIntro);
            storyImage = GameObject.Find("Story").GetComponent<SpriteRenderer>().gameObject;
            skipIntroButton = GameObject.Find("SkipIntroButton");
            storyImage.GetComponent<SpriteRenderer>().gameObject.SetActive(true);
            StoryTelling();
        }
        else
        {
            skipIntro = true;
            levelImage.SetActive(true);
            GameObject.Find("SkipIntroButton").SetActive(false);
            GameObject.Find("Story").SetActive(false);
            Invoke("HideLevelImage", levelStartDelay);
        }

        boardScript.SetupScene(level);

    }

    public void StoryTelling()
    {
        StartCoroutine(storyWaiter());    
    }

    IEnumerator storyWaiter()
    {
        int count = 1;
        while ((count < 6) && (skipIntro == false))
        {
            count++;
            //yield return new WaitForSeconds(6);
            yield return new WaitForSeconds(2);
            if(skipIntro == false)
            {
                yield return new WaitForSeconds(2);
            }
            if (skipIntro == false)
            {
                yield return new WaitForSeconds(2);
            }

            ChangePhoto(count);
        }
        if ((count >= 6) || skipIntro == true)
        {
            skipIntroButton.SetActive(false);
            storyImage.GetComponent<SpriteRenderer>().gameObject.SetActive(false);

            levelImage.SetActive(true);
            Invoke("HideLevelImage", levelStartDelay);
        }
       

    }

    public void ChangePhoto(int count)
    {
        Sprite newSprite = Resources.Load<Sprite>("story" + count.ToString());
        GameObject storyimg = GameObject.Find("Story");
        storyimg.GetComponent<SpriteRenderer>().sprite = newSprite;
    }


    public void HideLevelImage()
    {
        levelImage.SetActive(false);
        viewSeeWorldButton.GetComponent<Button>().gameObject.SetActive(true);
        viewSeeWorldButton.GetComponent<Button>().interactable = true;
        BoardManager.HideElements(true, 1);

        showLevelProgressText = true;

        doingSetup = false;
    }



    public void DisableButton()
    {
        if (GameManager.instance.remainingLevelViews <= 0)
        {
             viewSeeWorldButton.GetComponent<Button>().interactable = false;
        }

       
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
        godImage.SetActive(true);

        viewSeeWorldButton.GetComponent<Button>().gameObject.SetActive(false);

        enabled = false;
        BoardManager.move = false;
        StartCoroutine(waitForLevel());
    }

    public void StartOver()
    {
        enabled = false;
        SoundManager.instance.music2Source.Stop();
        SceneManager.LoadScene(0);

        LevelChanger.buttonClicked = false;
        Destroy(gameObject);
        SoundManager.instance.music1Source.Play();


    }

    public void CompletedGame()
    {

        wonImage.SetActive(true);
        enabled = false;

        StartCoroutine(waiter());

    }


    IEnumerator waitForLevel()
    {
        yield return new WaitForSeconds(3);
        godImage.SetActive(false);
        levelImage.SetActive(true);
        StartCoroutine(waiter());


    }

    IEnumerator waiter()
    {
        //when player hits enemy and dies, the game waits three seconds and then passes back to main menu. Also makes sure to destroy the gameobject so it will create a fresh one.
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);

        LevelChanger.buttonClicked = false;
        Destroy(gameObject);
        SoundManager.instance.music1Source.Play();
    }

}
