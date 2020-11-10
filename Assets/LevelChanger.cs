using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChanger : MonoBehaviour

{
    public Animator animator;
    private int levelToLoad;

    public GameObject gameManager;            //GameManager prefab to instantiate.


    // Start is called before the first frame update

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("Fadeout");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FadeToLevel(0);
            Debug.Log("lets go to the game");
           // GameManager.instance.initGame = true;
            if (GameManager.instance == null)

                //Instantiate gameManager prefab
                Instantiate(gameManager);

        }
    }

    public void OnFadeComplete()
    {
        Debug.Log("maybe this is the problem :0 " + levelToLoad);
        SceneManager.LoadScene(levelToLoad);
    }
}
