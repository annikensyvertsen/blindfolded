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

    private int lives; 

    public Text lifeText;

    private bool seeWorld = false;

    // Start is called before the first frame update
    protected override void Start()
    {
       
        animator = GetComponent<Animator>();

        lives = GameManager.instance.playerLives;

        seeWorld = GameManager.instance.seeWorld; 

        lifeText.text = "Lives: " + lives;


        base.Start();
    }

    private void OnDisable()
    {
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
        if (keyCounter == 1 )
        {
            int horizontal = 0;
            int vertical = 0;

            //Input.GetAxisRaw: Returns the value of the virtual axis identified by axisName with no smoothing filtering applied.
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));


            if (horizontal != 0)
            {
                vertical = 0;

            }

            if (horizontal != 0 || vertical != 0 )

            {
                AttemptMove(horizontal, vertical);

            }
        }
    }

    protected override void AttemptMove (int xDir, int yDir)
    {
        //every time the player moves, steps are added. 
        //xDir = 1 if you move one step to the right, -1 if you move to the left
 
        if (BoardManager.move == true)
        {
            CheckIfSomethingIsHit(xDir, yDir);

            base.AttemptMove(xDir, yDir);
            CheckIfGameOver();
        }
        

    }

    private void CheckIfSomethingIsHit(int xDir, int yDir)
    {
        List<Vector3> starPositions = BoardManager.starPositions;
        List<Vector3> enemyPositions = BoardManager.enemyPositions;
        
        List<Vector3> toBeRemoved = new List<Vector3>();


        Vector2 now = MovingObject.posNow + new Vector2(xDir, yDir);

        starPositions.ForEach(position =>
        {
            if(position[0] == now[0] && position[1] == now[1])
            {
                BoardManager.HideElements(true, 1);
                toBeRemoved.Add(position);
            }
        });
        enemyPositions.ForEach(position =>
        {
            if (position[0] == now[0] && position[1] == now[1])
            {
                BoardManager.HideElements(true, 1);
                toBeRemoved.Add(position);
            }
        });
        toBeRemoved.ForEach(elementToRemove =>
        {
            starPositions.Remove(elementToRemove);
        });

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
            //you should only get more possibilites to see the world if you have less than three
            if(GameManager.instance.remainingLevelViews < 3)
            {
                GameManager.instance.remainingLevelViews += 1;
                GameManager.instance.viewLevelText.text = GameManager.instance.remainingLevelViews + "/3";
            }
            
            //Disable the star object the player collided with.

            other.gameObject.SetActive (false); 
        }
        else if (other.tag == "Enemy")
        {
            lives--;
            lifeText.text = "Lives: " + lives;
        }
    }


    private void Restart()
    {
        SceneManager.LoadScene(0);
    }


    private void CheckIfGameOver()
    {
        if (lives <= 0)
            GameManager.instance.GameOver();
    }
}
