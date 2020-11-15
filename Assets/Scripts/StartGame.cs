using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip clickSound;

    public void TaskOnClick()
    {
        SoundManager.instance.PlaySingle(clickSound);

        LevelChanger.buttonClicked = true;
        SoundManager.instance.music1Source.Stop();
        SoundManager.instance.music2Source.Play();

    }


}
