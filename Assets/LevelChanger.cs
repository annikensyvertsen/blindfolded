using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChanger : MonoBehaviour

{
    public Animator animator;
    private int levelToLoad;

    public GameObject gameManager;            //GameManager prefab to instantiate.

    public static bool buttonClicked = false;
    public static bool loadStartPage = false;


    // Start is called before the first frame update

    public void FadeToLevel(int levelIndex)
    {
        levelToLoad = levelIndex;
        animator.SetTrigger("Fadeout");

    }

    // Update is called once per frame
    void Update()
    {
        if(buttonClicked == true)
        {
            //FadeToLevel(0);

            FadeToLevel(1);
            if (GameManager.instance == null)
                //Instantiate gameManager prefab
                Instantiate(gameManager);

        }
        if(loadStartPage == true)
        {
            //FadeToLevel(1);

            FadeToLevel(0);
        }
    
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
