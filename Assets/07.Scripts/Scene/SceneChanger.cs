using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // ���ο� ���� �񵿱������� �ε�
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);

        // �ε尡 �Ϸ�� ������ ���
        while (!loadOperation.isDone)
        {
            yield return null; // ���� �����ӱ��� ���
        }
    }
}