using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // �����̴� ����� ���� �߰�

public class MusicManager : MonoBehaviour
{
    public AudioClip startSceneMusic;
    public AudioClip lobbySceneMusic;
    public AudioClip stage1Music;
    public AudioClip stage2Music;
    public AudioClip stage3Music;
    public AudioClip stage4Music;
    public AudioClip stage5Music;

    private AudioSource audioSource;

    public Slider volumeSlider; // ���� ������ �����̴� UI

    void Awake()
    {
        // �� ��ȯ �ÿ��� �� ������Ʈ�� �������� �ʵ��� ����
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // AudioSource ������Ʈ�� ������ �߰�
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ���� �ʱ�ȭ (�����̴� ���� ������ �⺻ �� 1�� ����)
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f); // ����� ���� ���� �ҷ���
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged); // �����̴� �� ���� �� ȣ��� �Լ� ���
        }

        // ���� ����
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        // �� �ε� �ø��� ������ ����
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "StartScene":
                clipToPlay = startSceneMusic;
                break;
            case "LobbyScene":
                clipToPlay = lobbySceneMusic;
                break;
            case "Ch1_Stage_01":
                clipToPlay = stage1Music;
                break;
            case "Ch1_Stage_02":
                clipToPlay = stage2Music;
                break;
            case "Ch1_Stage_03":
                clipToPlay = stage3Music;
                break;
            case "Ch1_Stage_04":
                clipToPlay = stage4Music;
                break;
            case "Ch1_Stage_05":
                clipToPlay = stage5Music;
                break;
        }

        if (clipToPlay != null && audioSource != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    // ���� �����̴� �� ���� �� ȣ��Ǵ� �Լ�
    void OnVolumeChanged(float value)
    {
        audioSource.volume = value; // AudioSource�� ������ �����̴� ������ ����
        PlayerPrefs.SetFloat("Volume", value); // ���� ���� ����
    }
}
