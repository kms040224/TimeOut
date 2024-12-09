using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;  // 슬라이더 사용을 위해 추가

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

    public Slider volumeSlider; // 볼륨 조절용 슬라이더 UI

    void Awake()
    {
        // 씬 전환 시에도 이 오브젝트가 삭제되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // AudioSource 컴포넌트가 없으면 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 볼륨 초기화 (슬라이더 값이 없으면 기본 값 1로 설정)
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f); // 저장된 볼륨 값을 불러옴
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged); // 슬라이더 값 변경 시 호출될 함수 등록
        }

        // 음악 시작
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        // 씬 로딩 시마다 음악을 변경
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

    // 볼륨 슬라이더 값 변경 시 호출되는 함수
    void OnVolumeChanged(float value)
    {
        audioSource.volume = value; // AudioSource의 볼륨을 슬라이더 값으로 설정
        PlayerPrefs.SetFloat("Volume", value); // 볼륨 값을 저장
    }
}
