using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance; // ½Ì±ÛÅæ ÀÎ½ºÅÏ½º
    private AudioSource audioSource;

    public AudioClip startSceneBGM; // ½ºÅ¸Æ® ¾À BGM
    public AudioClip lobbySceneBGM; // ·Îºñ ¾À BGM

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // BGM Manager À¯Áö
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
    }

    // BGM º¯°æ ÇÔ¼ö
    public void PlayBGM(AudioClip clip)
    {
        if (audioSource.clip == clip)
            return;

        audioSource.clip = clip;
        audioSource.Play();
    }
}