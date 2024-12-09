using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // 게임 정지 해제
        SceneManager.LoadScene("StartScene"); // 메인 메뉴 씬으로 이동
    }
}