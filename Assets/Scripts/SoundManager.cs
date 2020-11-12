using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource efxSource;
    public AudioSource music1Source;
    public AudioSource music2Source;

    public static SoundManager instance = null;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        else if(instance != null)
        {
            Destroy(gameObject);
        };
        DontDestroyOnLoad(gameObject);
        
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }
}
