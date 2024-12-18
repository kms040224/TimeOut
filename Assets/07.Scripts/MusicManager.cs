using System.Collections;
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
    public AudioClip BossSceneMusic;

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

        // ���� ����
        PlayMusicForScene(SceneManager.GetActiveScene().name);

        // ���� �ʱ�ȭ (�����̴� ���� ������ �⺻ �� 1�� ����)
        if (volumeSlider != null)
        {
            float storedVolume = PlayerPrefs.GetFloat("Volume", 1f); // ����� ���� �� �ҷ�����
            volumeSlider.value = storedVolume; // �����̴� �� ����
            audioSource.volume = storedVolume; // ����� �ҽ� ���� ����

            // �����̴� �� ���� �� ȣ��� �Լ� ���
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
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
        // �� �ε� �� ������ ����
        PlayMusicForScene(scene.name);

        // �� �ε� �� ù ��°�� ã�� Slider UI�� ���
        if (volumeSlider == null)
        {
            volumeSlider = FindObjectOfType<Slider>();

            if (volumeSlider == null)
            {
                Debug.LogError("Slider not found in the scene!");
                return;  // �����̴��� ������ �� �̻� �������� ����
            }
        }

        // �����̴��� �����ϸ� ������ ����
        if (volumeSlider != null)
        {
            // ����� ���� ���� �ҷ��ͼ� �����̴��� ����� �ҽ��� �ݿ�
            float storedVolume = PlayerPrefs.GetFloat("Volume", 1f);
            volumeSlider.value = storedVolume;
            audioSource.volume = storedVolume;

            // �����̴� �� ���� �� ȣ��� �Լ� ��� (�ߺ� ����� ���ϱ� ���� ���� ������ ����)
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

    // ���� �����̴� �� ���� �� ȣ��Ǵ� �Լ�
    void OnVolumeChanged(float value)
    {
        // �����̴� ���� ����� �ҽ� ������ �ݿ�
        audioSource.volume = value;

        // ����� ���� PlayerPrefs�� ����
        PlayerPrefs.SetFloat("Volume", value);
    }
}