using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionsButton : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject instructionsUI;

    [SerializeField] private bool isPaused;


    public void TaskOnClick()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            ActivateInstructions();
        }
    }
    public void QuitInstructions()
    {
        DeactivateInstructions();
    }

    public void ActivateInstructions()
    {
        Time.timeScale = 0;
        instructionsUI.SetActive(true);
    }

    public void DeactivateInstructions()
    {
        Time.timeScale = 1;
        instructionsUI.SetActive(false);
        isPaused = false;
    }
}