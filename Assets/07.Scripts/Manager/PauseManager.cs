using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // �Ͻ����� �޴� UI (�г�)

    private bool isPaused = false; // ������ �Ͻ����� �������� Ȯ���ϴ� ����

    void Update()
    {
        // ESC Ű�� ������ �� �Ͻ�����/�簳
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // ���� �簳
            }
            else
            {
                PauseGame(); // ���� �Ͻ�����
            }
        }
    }

    // ������ �簳�ϴ� �Լ�
    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // �Ͻ����� �޴� ��Ȱ��ȭ
        Time.timeScale = 1f; // �ð� �帧 �簳
        isPaused = false;
    }

    // ������ �Ͻ������ϴ� �Լ�
    void PauseGame()
    {
        pauseMenuUI.SetActive(true); // �Ͻ����� �޴� Ȱ��ȭ
        Time.timeScale = 0f; // �ð� �帧 ����
        isPaused = true;
    }

    // ���� ���� ��ư (�Ͻ����� �޴��� ����)
    public void QuitGame()
    {
        Debug.Log("���� ����");
        Application.Quit(); // ���� ����
    }
}