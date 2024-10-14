using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public int areaIndex; // ���� ��ȣ
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ Ʈ���ſ� ������
        {
            gameManager.OnPlayerEnterArea(areaIndex); // �ش� ������ ���� ����

            // ���� ������ ���� �ٽ� Ȱ��ȭ
            if (areaIndex > 0) // ù ��° ������ �ƴ� ���
            {
                gameManager.ToggleDoor(areaIndex - 1, true); // ���� ���� Ȱ��ȭ
            }
        }
    }
}