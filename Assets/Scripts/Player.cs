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
    public int level = 1;

    private Text levelText;

    private bool seeWorld = false;

    // Start is called before the first frame update
    public AudioClip moveSound;
    public AudioClip catchRatSound;
    public AudioClip walkOnEnemySound;
    public AudioClip gameOverSound;



    protected override void Start()
    {
        animator = GetComponent<Animator>();


        lives = GameManager.instance.playerLives;
        level = GameManager.instance.levels;

        seeWorld = GameManager.instance.seeWorld;

        levelText = GameObject.Find("LevelProgressText").GetComponent<Text>();

        levelText.text = "Level: " + level ;

        if (GameManager.instance.remainingLevelViews <= 0)
        {
            GameManager.instance.DisableButton();
        }
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
            bool hitSomething = CheckIfSomethingIsHit(xDir, yDir);
            if (hitSomething)
            {
                GameManager.instance.playerLives--;
                CheckIfGameOver();


            }

            animator.SetTrigger("jump");
            SoundManager.instance.PlaySingle(moveSound);

            base.AttemptMove(xDir, yDir);
        }


    }

    private bool CheckIfSomethingIsHit(int xDir, int yDir)
    {
        List<Vector3> starPositions = BoardManager.starPositions;
        List<Vector3> enemyPositions = BoardManager.enemyPositions;
        
        List<Vector3> toBeRemoved = new List<Vector3>();

        bool hitAStar = false;
        bool hitAnEnemy = false;

        Vector2 now = MovingObject.posNow + new Vector2(xDir, yDir);
        //sometimes it does not convert to float on the first move, and since the player has position "x.3f" to be high enough on the board, we have to double check here
        if(now[1] is int || now[1] == 0 || (now[1] is float && now[1] == 1.0f))
        {
            now[1] = ((float)((float)(now[1]) + 0.3f));
        };

        starPositions.ForEach(position =>
        {
            if ((position[0] == now[0]) && ((float)((float)(position[1]) + 0.3f) == (float)now[1]))
            {
                
                BoardManager.HideElements(true, 1);
                hitAStar = true;
                toBeRemoved.Add(position);
                Debug.Log("catch rat");
                SoundManager.instance.PlaySingle(catchRatSound);

                GameObject viewSeeWorldButton = GameObject.Find("SeeWorldButton");
                viewSeeWorldButton.GetComponent<Button>().enabled = true;
                viewSeeWorldButton.GetComponent<Button>().interactable = true;
            }
        });

        enemyPositions.ForEach(position =>
        {
            /*Debug.Log("enemyposition? " + position + now);
            Debug.Log("is it true? " + ((position[0] == now[0]) && ((float)((float)(position[1]) + 0.3f) == (float)now[1])));
            Debug.Log("first? " + (position[0] == now[0]));
            Debug.Log("second? " + ((float)((float)(position[1]) + 0.3f) == (float)now[1]));*/

            if ((position[0] == now[0]) && (((float)((float)(position[1]) + 0.3f) == (float)now[1])) || (position[1] == now[1]))
            {
                hitAnEnemy = true;
                BoardManager.HideElements(true, 1);
                toBeRemoved.Add(position);
                SoundManager.instance.PlaySingle(walkOnEnemySound);
            }
                
        });
        toBeRemoved.ForEach(elementToRemove =>
        {
            if (hitAStar)
                starPositions.Remove(elementToRemove);
            else if (hitAnEnemy)
                enemyPositions.Remove(elementToRemove);
        });
        if (hitAnEnemy)
        {
            Debug.Log("hit");
            return true;
        }
        return false;

    }


private void OnTriggerEnter2D (Collider2D other)
    {
        if(other.tag == "Door")
        {
           // Invoke("Restart", restartLevelDelay);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (GameManager.instance.remainingLevelViews <= 0)
            {
                GameManager.instance.DisableButton();
            }
        }
        else if(other.tag == "Star")
        {
            //you should only get more possibilites to see the world if you have less than three
            if(GameManager.instance.remainingLevelViews < 3)
            {
                GameManager.instance.remainingLevelViews += 1;
            }

            //Disable the star object the player collided with.
            other.gameObject.SetActive(false); 
        }
        else if (other.tag == "Enemy")
        {
            GameManager.instance.playerLives--;
            lives--;
            StartCoroutine(waiter());

            levelText.text = "Level: " + level;
        }
    }
    IEnumerator waiter()
    {
        yield return new WaitForSeconds(1);
        CheckIfGameOver();
    }


    private void Restart()
    {
        //SceneManager.LoadScene(0);
        GameManager.instance.remainingLevelViews = 3;
        SceneManager.LoadScene(1);
    }


    private void CheckIfGameOver()
    {
        if (GameManager.instance.playerLives <= 0)
        {
            GameManager.instance.GameOver();
            SoundManager.instance.music2Source.Stop();
        }
          
    }
}
