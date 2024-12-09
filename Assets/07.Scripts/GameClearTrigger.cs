using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearTrigger : MonoBehaviour
{
    public GameObject gameClearPanel; // ���� Ŭ���� �г�

    private void Start()
    {
        if (gameClearPanel != null)
        {
            gameClearPanel.SetActive(false); // ���� ���� �� �г� ����
        }
        else
        {
            Debug.LogError("GameClearPanel is not assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹 �� ���� Ŭ���� �г� ǥ��
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player collided with GameClearTrigger. Displaying Game Clear Panel.");
            if (gameClearPanel != null)
            {
                gameClearPanel.SetActive(true);
                Time.timeScale = 0f; // ���� ����
            }
        }
    }
}