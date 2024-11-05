using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // �̵��� �� �̸�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ��Ż�� �����ϸ�
        {
            LoadNextScene();
        }
    }

    // ���� ���� �ε��ϴ� �Լ�
    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad); // ���� ������ ��ȯ
    }
}