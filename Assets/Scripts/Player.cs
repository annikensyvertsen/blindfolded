using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public int pointsPerStar = 1;
    public float restartLevelDelay = 1f;

    private Animator animator;


    private int stars;
    private int lives; 
    private int steps;

    public Text stepText;

    public Text starText;

    public Text lifeText;


    // Start is called before the first frame update
    protected override void Start()
    {
       
        animator = GetComponent<Animator>();

        stars = GameManager.instance.playerStarPoints;
        lives = GameManager.instance.playerLives;
        steps = GameManager.instance.playerSteps; 

        starText.text = "Stars: " + stars;
        stepText.text = "Steps: " + steps;
        lifeText.text = "Lives: " + lives; 

        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerStarPoints = stars;
        GameManager.instance.playerLives = lives; 
    }
    // Update is called once per frame
   


    void Update()
    {
        //check that it can only move one step at a time
        int keyCounter = 0; 

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            keyCounter ++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            keyCounter++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            keyCounter++;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            keyCounter++;
        }
        if (keyCounter == 1)
        {

            //Debug.Log("player position: " + GameObject.Find("Player").transform.position);
            //Debug.Log("position 1: " + GameObject.Find("Player").transform.position[1]);
            Debug.Log("input string: " + Input.inputString);
            foreach (char c in Input.inputString)
            {
                Debug.Log("char: " + c); 
            }


            int horizontal = 0;
            int vertical = 0;

            //Input.GetAxisRaw: Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));

            if (horizontal != 0)
            {
                vertical = 0;

            }

            if (horizontal != 0 || vertical != 0)
            {
                AttemptMove(horizontal, vertical);

            }
        }
     

        


    }

    protected override void AttemptMove (int xDir, int yDir)
    {
        //every time the player moves, steps are added. 
        
        steps++;

        //TODO: sjekke her eller i update eller i move -> slik at den ikke går ut av banen

        stepText.text = "Steps: " + steps;

        base.AttemptMove(xDir, yDir);
        RaycastHit2D hit;
        CheckIfGameOver();

    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.tag == "Door")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if( other.tag == "Star")
        {

            stars += 1;
            starText.text = "Stars: " + stars;
            //Disable the star object the player collided with.

            other.gameObject.SetActive (false); 
        }
        else if (other.tag == "Enemy")
        {
            lives--;
            lifeText.text = "Lives: " + lives;
        }
    }

    protected override void OnCantMove <T> (T component)
    {
        //in the tutorial it checks if it hits inner wall here, we dont have inner walls
    }

    private void Restart()
    {
        //Application.LoadLevel(Application.loadedLevel);
        SceneManager.LoadScene(0);
    }
    public void CollectStars()
    {
        stars += 1;
        starText.text = "+ " + 1 + "Stars: " + stars;
    }

    public void LoseStars(int loss)
    {
        stars -= loss;
        starText.text = "- " + loss + "Stars: " + stars;

        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (lives <= 0)
            GameManager.instance.GameOver();
        //if (stars <= 0)
          //  GameManager.instance.GameOver();
    }
}
