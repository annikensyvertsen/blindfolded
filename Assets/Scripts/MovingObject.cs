using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.05f; //time it will take for the object to move (seconds)
    public LayerMask blockingLayer; //collision layer

    private BoxCollider2D boxCollider; 
    private Rigidbody2D rb2D;
    private float inverseMoveTime; //used to make movement more efficient

    public Vector3 spawn;



    // Start is called before the first frame update
    protected virtual void Start()
    {
        //set the start position of the player
        spawn = new Vector3(0, 0, 0);
        transform.position = spawn;

        //get component references to BoxCollider2D and Rigidbody2D
        boxCollider = GetComponent<BoxCollider2D>(); 
        rb2D = GetComponent <Rigidbody2D>();

        //by storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient
        inverseMoveTime = 1f / moveTime;
    }

    //move returns true if it is able to move and false if not
    //takes parameters for x direction, y directin and raycasthit3d to check collision
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        //store start position to move from, based on objects current transformation position
        Vector2 start = transform.position;
        //calculate end position based on the direction parameters passed in when calling move
        Vector2 end = start + new Vector2 (xDir, yDir);

        end[0] = (int)end[0];
        end[1] = (int)end[1];

        //disable boxCollider so linecast doesn't hit this objects own collider
        boxCollider.enabled = false;

        //cast a line from start point to end point by checking collision on blockinglayer
        hit = Physics2D.Linecast(start, end, blockingLayer);
        //BoardManager.enemyPositions.ForEach(p => Debug.Log("enemy p: " + p));
        if (end[0] == -1 || end[1] == -1 || end[0] == (BoardManager.columns) || end[1] == (BoardManager.rows))
        {
            return false;
        }

        //re-enable boxCollider after linecast
        boxCollider.enabled = true;
        //check if you are at the edge of the board
        
        //check if anything was hit
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement (end));

            //return true to say that move was successfull
            return true;
        }
        //if something was hit return false
        return false;
    }

    //co-routine for moving units from one space to next, takes a parameter ent to specify where to move to
    protected IEnumerator SmoothMovement (Vector3 end)
    {

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {

            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);

            rb2D.MovePosition (newPosition);

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            //wait for frame before reevaluating condition of loop
            yield return null;
        }
    }


    protected virtual void AttemptMove(int xDir, int yDir) 
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;
    }


    protected abstract void OnCantMove <T> (T component) where T : Component;

    // Update is called once per frame
    void Update()
    {
       
    }
}
