using System.Collections;
using System.Collections.Generic;
using System; 
using UnityEngine;
using Random = UnityEngine.Random; 

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum; // min value for count class
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    public static int columns = 8;
    public static int rows = 8;

    public Count starCount = new Count(1, 2);

    public GameObject door;

    public GameObject[] floorTiles;
    public static GameObject[] hideFloorTiles;

    public GameObject[] wallTiles;
    public GameObject[] starTiles;
    public GameObject[] enemyTiles;

    private static GameObject[] starCopyTiles;
    private static GameObject[] enemyCopyTiles;

    private static Transform boardHolder;
    private static Transform starHolder;
    private static Transform enemyHolder;

    private bool visibility; 

    private List<Vector3> gridPositions = new List<Vector3> (); // a list of possible locations to place tiles
    //private bool seeWorld = GameManager.instance.seeWorld; 


    void InitialiseList() //clears our list gridpositions and prepares it to generate new board
    {
        gridPositions.Clear();
        for (int x = 1; x < columns; x++) //loop throug x-axis/columns
        {
            for (int y = 1; y < rows; y++) // whithin each loop, loop through y axis (rows)
            {
                gridPositions.Add(new Vector3(x, y, 0f)); // at each index, add a new vector3 to our list with the x and y coordinates of that position
            }
        }
    }

    void BoardSetup() //sets up floor of the game board
    {
        boardHolder = new GameObject ("Board").transform;
        starHolder = new GameObject("Star").transform;
        enemyHolder = new GameObject("Enemy").transform;


        for (int x = 0; x < columns; x+=1) // loop along x axis, starting from 0 (to fill corner) with floor or outerwall edge tiles
        {
            for (int y = 0; y < rows; y+=1) //loop along y-axis, starting from 0 to place floor tile prefabs and prepare to instansiate it
            {
                GameObject toInstantiate = floorTiles[0];

                GameObject instance = Instantiate (toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //instantiate the gameobject instance using the prefab chosen for toInstantiate at the vector 3
                //corresponing to current grid position in loop, cast it to GameObject
                instance.transform.SetParent(boardHolder);

            }
        }    
    }

    public static bool move = true;


    //this function is supposed to hide and show elements when the user clicks the "seeWorld" button, but only when three levels
    public static void HideElements(bool seeWorld, int timer = 3)
    {
        //visibility = seeWorld;
        //should be called with true at the start
        if (seeWorld == true)
        {
            move = false;
            //show elements
            starPositions.ForEach(tile =>
            {
                GameObject tileChoice = starCopyTiles[Random.Range(0, starCopyTiles.Length)];
                GameObject instance = Instantiate(tileChoice, tile, Quaternion.identity) as GameObject;
                instance.transform.SetParent(starHolder);
            });
            enemyPositions.ForEach(tile =>
            {
                GameObject tileChoice = enemyCopyTiles[Random.Range(0, enemyCopyTiles.Length)];

                //og her
                GameObject instance = Instantiate(tileChoice, tile, Quaternion.identity) as GameObject;
                instance.transform.SetParent(enemyHolder);
            });
            GameManager.instance.timeRemaining = timer;
            GameManager.instance.timerIsRunning = true;

        }
        else
        {
            move = true;

            //hide elements 
            starPositions.ForEach(tile =>
            {
                float x = tile[0];
                float y = tile[1];
                foreach (Transform child in starHolder)
                {
                    if (child.position == tile)
                    {
                        Destroy(child.gameObject);
                    }
                }

            });
            enemyPositions.ForEach(tile =>
            {
                float x = tile[0];
                float y = tile[1];
                foreach (Transform child in enemyHolder)
                {
                    if (child.position == tile)
                    {
                        Destroy(child.gameObject);
                    }
                }
            });
            GameManager.instance.timerIsRunning = false;

        }
    }


    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);

        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }
    public static List<Vector3> enemyPositions = new List<Vector3>();
    public static List<Vector3> starPositions = new List<Vector3>();

 

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum, string type) //LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
    {
        //TODO - her må jeg gjøre en sjekk, sånn at verken stjerner eller miner blir plassert der man starter 
        int objectCount = Random.Range(minimum, maximum + 1);  
        

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;

            //checks if the type is enemies or stars, and adds it to the belonging position list
            if (type == "enemies")
            {
                enemyPositions.Add(randomPosition);
                enemyCopyTiles = tileArray;
                instance.transform.SetParent(enemyHolder);


            }
            if (type == "stars")
            {
                starPositions.Add(randomPosition);
                starCopyTiles = tileArray;
                instance.transform.SetParent(starHolder);


            }

        }
    }

    

    public void SetupScene(int level)
    {
        BoardSetup();

        InitialiseList();
        LayoutObjectAtRandom (starTiles, starCount.minimum, starCount.maximum, "stars");
        int enemyNumber = 1;
        if(level == 1)
        {
            enemyNumber = 0;
        }
        else
        {
            enemyNumber = level;
        }
        int enemyCount = enemyNumber * 2;
        LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount, "enemies");

        HideElements(false);


        Instantiate(door, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);

    }

}
