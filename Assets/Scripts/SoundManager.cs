using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource efxSource;
    public AudioSource efxSource2;

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

    public void PlayJump(AudioClip clip)
    {
        efxSource2.clip = clip;
        efxSource2.Play();
    }

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }
}
