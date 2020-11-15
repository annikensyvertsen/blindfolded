using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip seeWorldSound;
    public void TaskOnClick()
    {
        if (GameManager.instance.remainingLevelViews > 0)
        {
            SoundManager.instance.PlaySingle(seeWorldSound);
            GameManager.instance.remainingLevelViews--;
            GameManager.instance.seeWorld = !GameManager.instance.seeWorld;
            BoardManager.HideElements(GameManager.instance.seeWorld);
            BoardManager.move = false;

        }
        if(GameManager.instance.remainingLevelViews == 0)
        {
            GameManager.instance.DisableButton();
        }
    }
}
