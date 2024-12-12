using System.Collections;
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
    public AudioClip BossSceneMusic;

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

        // 음악 시작
        PlayMusicForScene(SceneManager.GetActiveScene().name);

        // 볼륨 초기화 (슬라이더 값이 없으면 기본 값 1로 설정)
        if (volumeSlider != null)
        {
            float storedVolume = PlayerPrefs.GetFloat("Volume", 1f); // 저장된 볼륨 값 불러오기
            volumeSlider.value = storedVolume; // 슬라이더 값 설정
            audioSource.volume = storedVolume; // 오디오 소스 볼륨 설정

            // 슬라이더 값 변경 시 호출될 함수 등록
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
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
        // 씬 로딩 시 음악을 변경
        PlayMusicForScene(scene.name);

        // 씬 로딩 시 첫 번째로 찾은 Slider UI를 사용
        if (volumeSlider == null)
        {
            volumeSlider = FindObjectOfType<Slider>();

            if (volumeSlider == null)
            {
                Debug.LogError("Slider not found in the scene!");
                return;  // 슬라이더가 없으면 더 이상 진행하지 않음
            }
        }

        // 슬라이더가 존재하면 볼륨을 설정
        if (volumeSlider != null)
        {
            // 저장된 볼륨 값을 불러와서 슬라이더와 오디오 소스에 반영
            float storedVolume = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.value = storedVolume;
            audioSource.volume = storedVolume;

            // 슬라이더 값 변경 시 호출될 함수 등록 (중복 등록을 피하기 위해 기존 리스너 제거)
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
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
            case "BossScene":
                clipToPlay = BossSceneMusic;
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
        // 슬라이더 값을 오디오 소스 볼륨에 반영
        audioSource.volume = value;

        // 변경된 값을 PlayerPrefs에 저장
        PlayerPrefs.SetFloat("Volume", value);
    }
}
