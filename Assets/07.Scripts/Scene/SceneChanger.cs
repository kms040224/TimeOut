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
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log("Changing to scene: " + sceneName); // �� ���� Ȯ�ο� �α�
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
            // �� �̸��� ��Ȯ���� Ȯ��
            ChangeScene("BossScene");  // �� �̸��� ��Ȯ�� �Է��ϼ���
        }
    }
}