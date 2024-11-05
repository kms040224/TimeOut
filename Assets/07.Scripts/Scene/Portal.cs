using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad; // 이동할 씬 이름

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 포탈에 접촉하면
        {
            LoadNextScene();
        }
    }

    // 다음 씬을 로드하는 함수
    void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad); // 다음 씬으로 전환
    }
}