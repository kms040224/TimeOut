using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public int areaIndex; // ���� ��ȣ
    private GameManager gameManager;
    private Collider areaCollider; // �ݶ��̴� ���� �߰�

    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager = GetComponent<GameManager>();
        gameManager = FindObjectOfType<GameManager>();
        areaCollider = GetComponent<Collider>(); // �ݶ��̴� ������Ʈ ��������
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ Ʈ���ſ� ������
        {
            GameManager.Instance.OnPlayerEnterArea(areaIndex); // �ش� ������ ���� ����

            // ���� ������ ���� �ٽ� Ȱ��ȭ
            if (areaIndex > 0) // ù ��° ������ �ƴ� ���
            {
                gameManager.ToggleDoor(areaIndex - 1, true); // ���� ���� Ȱ��ȭ
            }

            areaCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        }
    }
}
