using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // 일시정지 메뉴 UI (패널)

    private bool isPaused = false; // 게임이 일시정지 상태인지 확인하는 변수

    void Update()
    {
        // ESC 키를 눌렀을 때 일시정지/재개
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // 게임 재개
            }
            else
            {
                PauseGame(); // 게임 일시정지
            }
        }
    }

    // 게임을 재개하는 함수
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // 일시정지 메뉴 비활성화
        Time.timeScale = 1f; // 시간 흐름 재개
        isPaused = false;
    }

    // 게임을 일시정지하는 함수
    void PauseGame()
    {
        pauseMenuUI.SetActive(true); // 일시정지 메뉴 활성화
        Time.timeScale = 0f; // 시간 흐름 멈춤
        isPaused = true;
    }

    // 게임 종료 버튼 (일시정지 메뉴에 연결)
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit(); // 게임 종료
    }
}