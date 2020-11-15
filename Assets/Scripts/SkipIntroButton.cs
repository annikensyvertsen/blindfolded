using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipIntroButton : MonoBehaviour
{
    // Start is called before the first frame update
    public void TaskOnClick()
    {
        GameManager.instance.skipIntro = true;
    }
}
