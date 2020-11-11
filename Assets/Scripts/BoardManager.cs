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
    public static int columns = 6;
    public static int rows = 8;

    public Count starCount = new Count(1, 2);

    public GameObject door;

    public GameObject[] floorTiles;
    public static GameObject[] hideFloorTiles;

    public GameObject[] starTiles;
    public GameObject[] enemyTiles;

    public static bool move = true;

    private static GameObject[] starCopyTiles;
    private static GameObject[] enemyCopyTiles;

    private static Transform boardHolder;
    private static Transform starHolder;
    private static Transform enemyHolder;

    private bool visibility; 

    private List<Vector3> gridPositions = new List<Vector3> (); // a list of possible locations to place tiles
    private List<Vector3> gridPositionsCopy = new List<Vector3>(); // a list of possible locations to place tiles



    void InitialiseList() //clears our list gridpositions and prepares it to generate new board
    {
        gridPositions.Clear();
        for (int x = 0; x < columns; x++) //loop throug x-axis/columns
        {
            for (int y = 0; y < rows; y++) // whithin each loop, loop through y axis (rows)
            {
                gridPositions.Add(new Vector3(x, y, 0f)); // at each index, add a new vector3 to our list with the x and y coordinates of that position
            }
        }
        //fill list with tiles that are along the walls
        gridPositionsCopy.Clear();

        for (int x = 0; x < columns; x++) //loop throug x-axis/columns
        {
            for (int y = 0; y < rows; y++) // whithin each loop, loop through y axis (rows)
            {
                gridPositionsCopy.Add(new Vector3(x, y, 0f)); // at each index, add a new vector3 to our list with the x and y coordinates of that position

                //adds positions along the wall
                //if not start or end posititon
                if (!(x == 0 && y == 0) && !(x == columns - 1 && y == rows - 1))
                {
                 
                    if (x == 0 || x == columns - 1)
                    {
                        gridYPositions.Add(new Vector3(x, y, 0f));
                    }
                    if (y == 0 || y == rows - 1)
                    {
                        gridXPositions.Add(new Vector3(x, y, 0f));
                    }
                }
            }
        }
    }

    void BoardSetup() //sets up floor of the game board
    {
        boardHolder = new GameObject ("Board").transform;
        starHolder = new GameObject("Star").transform;
        enemyHolder = new GameObject("Enemy").transform;


        for (int x = 0; x < columns; x++) // loop along x axis, starting from 0 (to fill corner) with floor or outerwall edge tiles
        {
            for (int y = 0; y < rows; y++) //loop along y-axis, starting from 0 to place floor tile prefabs and prepare to instansiate it
            {
                GameObject toInstantiate = floorTiles[0];

                GameObject instance = Instantiate (toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject; //instantiate the gameobject instance using the prefab chosen for toInstantiate at the vector 3
                //corresponing to current grid position in loop, cast it to GameObject
                instance.transform.SetParent(boardHolder);
            }
        }
    }



    //this function is supposed to hide and show elements when the user clicks the "seeWorld" button, but only when three levels
    public static void HideElements(bool seeWorld, int timer = 3)
    {
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

        bool isStartOrEndTile = false;

        Vector3 randomPosition = gridPositions[randomIndex];

        if((randomPosition[0] == 0 && randomPosition[1] == 0)|| ((randomPosition[0] == (columns-1)) && (randomPosition[1] == (rows-1))))
        {
            isStartOrEndTile = true;
        }

        int count = 0;

        while(isStartOrEndTile == true)
        {
            randomPosition = gridPositions[randomIndex];

            if ((randomPosition[0] == 0 && randomPosition[1] == 0) || ((randomPosition[0] == (columns-1)) && (randomPosition[1] == (rows-1))))
            {
                isStartOrEndTile = true;
                randomIndex = Random.Range(0, gridPositions.Count);
                randomPosition = gridPositions[randomIndex];
            }
            else
            {
                isStartOrEndTile = false;
            }
            count++;
            if(count == 100)
            {
                isStartOrEndTile = false;
            }
        }
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    private List<Vector3> gridXPositions = new List<Vector3>();
    private List<Vector3> gridYPositions = new List<Vector3>();

    //returns a random postion along the edges of the board
    Vector3 RandomWallPosition(string direction)
    {
        
        int randomIndex;
        Vector3 randomPosition;

        if (direction == "x")
        {
            randomIndex = Random.Range(0, columns-1);
            randomPosition = gridXPositions[randomIndex];
            gridXPositions.RemoveAt(randomIndex);
        }
        else
        {
            randomIndex = Random.Range(0, rows-1);
            randomPosition = gridYPositions[randomIndex];
            gridYPositions.RemoveAt(randomIndex);
        }
        
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

            int count = 0;

            bool isInFreePath = false;
            if (type == "enemies")
            {
                //here we will try to make sure no enemies are placed in the free path
                if (path.Contains(randomPosition))
                {
                    isInFreePath = true;
                }
                while (isInFreePath == true)
                {
                    randomPosition = RandomPosition();
                    if (!(path.Contains(randomPosition)))
                    {
                        isInFreePath = false;
                    }
                    count++;
                    if (count == 100)
                    {
                        isInFreePath = false;
                    }

                }
            }

            GameObject instance = Instantiate(tileChoice, randomPosition, Quaternion.identity) as GameObject;

            //checks if the type is enemies or stars, and adds it to the belonging position list
            if (type == "enemies")
            {
                if (!enemyPositions.Contains(randomPosition))
                {
                    enemyPositions.Add(randomPosition);
                }
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

    //makes sure some enemies are created in random places along the walls to make it more difficult
    void LayoutObjectsAlongTheWalls(GameObject[] tileArray)
    {
        GameObject enemyTile = tileArray[Random.Range(0, tileArray.Length)];

        Vector3 wallXPosition = RandomWallPosition("x");

        bool xIsInFreePath = false;
        if (path.Contains(wallXPosition))
        {
            xIsInFreePath = true;
        }
        int xcount = 0;
        while (xIsInFreePath == true)
        {
            wallXPosition = RandomWallPosition("x");
            if (!(path.Contains(wallXPosition)))
            {
                xIsInFreePath = false;
            }
            xcount++;
            if(xcount == 100)
            {
                xIsInFreePath = false;
            }
        }

        Vector3 wallYPosition = RandomWallPosition("y");

        bool yIsInFreePath = false;
        int ycount = 0;
        if (path.Contains(wallYPosition))
        {
            yIsInFreePath = true;
        }
        while (yIsInFreePath == true)
        {
            wallYPosition = RandomWallPosition("y");
            if (!(path.Contains(wallYPosition)))
            {
                yIsInFreePath = false;
            }
            ycount++;
            if (ycount == 100)
            {
                yIsInFreePath = false;
            }
        }


        GameObject instanceX = Instantiate(enemyTile, wallXPosition, Quaternion.identity) as GameObject;
        GameObject instanceY = Instantiate(enemyTile, wallYPosition, Quaternion.identity) as GameObject;

        if (!enemyPositions.Contains(wallXPosition)){
            enemyPositions.Add(wallXPosition);
        }
        if (!enemyPositions.Contains(wallYPosition)){
            enemyPositions.Add(wallYPosition);
        }

        instanceX.transform.SetParent(enemyHolder);
        instanceY.transform.SetParent(enemyHolder);
        enemyCopyTiles = tileArray;
        
    }


    private List<Vector3> neighbourTiles = new List<Vector3>(); // 
    private List<Vector3> visitedTiles = new List<Vector3>(); // 
    private Vector3 currentNode = new Vector3(0, 0, 0);


    Vector3 returnRandomNeigbour(float currentX, float currentY)
    {

        //iterate through and add neighbournodes to the current node
        
        gridPositionsCopy.ForEach(pos =>
        {
            float neighbourX = pos[0];
            float neighbourY = pos[1];

            //if there is a neighbor on the x axis
            if (neighbourX == currentX)
            {
                //if there is a neighbour
                if (currentY + 1.0 == neighbourY)
                {

                    if (!neighbourTiles.Contains(pos) && !visitedTiles.Contains(pos))
                    {
                        neighbourTiles.Add(pos);
                    }
                }
            }
            //if there is a neighbor on the y axis
            if (neighbourY == currentY)
            {

                if (currentX + 1 == neighbourX || (currentX -1 == neighbourX && currentX == (columns-1)))
                {

                    if (!neighbourTiles.Contains(pos) && !visitedTiles.Contains(pos))
                    {

                        neighbourTiles.Add(pos);
                    }
                }
            }
        });


        //get a random index from the neigbours
        int lengthOfNeighbourTiles = neighbourTiles.Count;

        int randomNeighboorIndex = Random.Range(0, lengthOfNeighbourTiles);

        //choose a node of the neighbournodes with the random index and return it
        if(neighbourTiles.Count != 0)
        {
            return neighbourTiles[randomNeighboorIndex];
        }
        else
        {
            return currentNode;
        }
    }
    public List<Vector3> path = new List<Vector3>(); // 

    //since the boards are randomly generated for each level, we need to make sure there is always a free path with no enemies from start to end
    void CreateRandomPathToGoal()
    {
        //TODO : fikse denne indekseringen
        Vector3 start = new Vector3(0, 0, 0);
        Vector3 end = new Vector3(columns-1, rows-1, 0);

        visitedTiles.Add(currentNode);
        bool reachedGoal = false;
        int count = 0;
        path.Add(currentNode);
      
        while (reachedGoal == false)
        {
            float currentX = currentNode[0];
            float currentY = currentNode[1];



            //returns a Vector3 with a random neighbour of the assigned node
            Vector3 randomNeighbour = returnRandomNeigbour(currentX, currentY);

            //check that we have not already been at that tile
            if (visitedTiles.Contains(randomNeighbour))
            {
                randomNeighbour = returnRandomNeigbour(currentX, currentY);
            }
            //assign current node to the random value
            currentNode = randomNeighbour;
            gridPositionsCopy.Remove(currentNode);


            //add it to a list so that the same wont be choosen so we dont get a loop
            visitedTiles.Add(currentNode);

            path.Add(currentNode);
            count++;
            //check if we have reached goal

            if (currentNode == end)
            {
                reachedGoal = true;
            }
            neighbourTiles = new List<Vector3>();

            if (count == 100)
            {
   
                reachedGoal = true;
            }
           
            //pick a random neighboor in the nodes neighboor list
            //int randomNeighboor = Random.Range(0, neighbourTiles.Count);

        }


    }

    void CheckIfGameIsWon()
    {
          if (GameManager.instance.levels == 14)
        {
            GameManager.instance.CompletedGame();
        }
    }

    public void SetupScene(int level)
    {
        CheckIfGameIsWon();
        //empty all lists at start of each level
        neighbourTiles = new List<Vector3>(); 
        visitedTiles = new List<Vector3>();
        path = new List<Vector3>();
        currentNode = new Vector3(0, 0, 0);

        BoardSetup();
        InitialiseList();

        CreateRandomPathToGoal();

        if (GameManager.instance.remainingLevelViews < 3)
        {
            LayoutObjectAtRandom(starTiles, 1, 1, "stars");

        }
        LayoutObjectsAlongTheWalls(enemyTiles);

        HideElements(true);

        gridPositions.ForEach(f =>
        {
            Debug.Log("f: " + f);
        });

        int enemyNumber = 1;
        if(level == 1)
        {
            enemyNumber = 0;
        }
        else
        {
            if(level < 13)
            {
                enemyNumber = level;
            }
            else
            {
                enemyNumber = 12;
            }
        }
        int enemyCount = enemyNumber * 2;
        LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount, "enemies");

        Instantiate(door, new Vector3(columns - 1, rows - 1, 0F), Quaternion.identity);

    }

}
