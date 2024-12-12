using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log("Changing to scene: " + sceneName); // 씬 변경 확인용 로그
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // 씬 이름이 정확한지 확인
            ChangeScene("BossScene");  // 씬 이름을 정확히 입력하세요
        }
    }
}