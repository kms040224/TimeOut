using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearPanel : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // ���� ���� ����
        SceneManager.LoadScene("StartScene"); // ���� �޴� ������ �̵�
    }
}