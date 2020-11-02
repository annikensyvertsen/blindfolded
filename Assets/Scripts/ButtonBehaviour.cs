using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    // Start is called before the first frame update


    public void TaskOnClick()
    {
        if (GameManager.instance.remainingLevelViews > 0)
        {
            GameManager.instance.remainingLevelViews--;
            int count = GameManager.instance.remainingLevelViews;
            GameManager.instance.viewLevelText.text = count + "/3";
            GameManager.instance.seeWorld = !GameManager.instance.seeWorld;
            BoardManager.HideElements(GameManager.instance.seeWorld);
        }
    }
}
