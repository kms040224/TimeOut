using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearTrigger : MonoBehaviour
{
    public GameObject gameClearPanel; // 게임 클리어 패널

    private void Start()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(false); // 게임 시작 시 패널 숨김
        }
        else
        {
            Debug.LogError("GameClearPanel is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌 시 게임 클리어 패널 표시
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with GameClearTrigger. Displaying Game Clear Panel.");
            if (gameClearPanel != null)
            {
                gameClearPanel.SetActive(true);
                Time.timeScale = 0f; // 게임 정지
            }
        }
    }
}