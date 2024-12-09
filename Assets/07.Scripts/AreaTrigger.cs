using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public int areaIndex; // ���� ��ȣ
    private GameManager gameManager;
    private Collider areaCollider; // �ݶ��̴� ����

    void Start()
    {
        // ������ GameManager�� ã�Ƽ� �Ҵ�
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager �ν��Ͻ��� ã�� �� �����ϴ�!");
            return; // ������ ������ �� �����Ƿ� ����
        }

        // �ݶ��̴� ������Ʈ ��������
        areaCollider = GetComponent<Collider>();
        if (areaCollider == null)
        {
            Debug.LogError("Collider�� AreaTrigger ������Ʈ�� �����ϴ�!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (areaCollider == null)
        {
            Debug.LogError($"AreaCollider�� null�Դϴ�. AreaTrigger {areaIndex}�� Ȯ���ϼ���.");
            return;
        }

        Debug.Log($"Collider {other.name} entered AreaTrigger {areaIndex}");

        // �÷��̾ Ʈ���ſ� ������ �� ó��
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered Area {areaIndex}");
            gameManager.OnPlayerEnterArea(areaIndex);

            // ���� ������ �� Ȱ��ȭ
            if (areaIndex > 0)
            {
                gameManager.ToggleDoor(areaIndex - 1, true);
                Debug.Log($"Door for area {areaIndex - 1} has been enabled.");
            }

            // Ʈ���� �ݶ��̴� ��Ȱ��ȭ
            areaCollider.enabled = false;
            Debug.Log($"Area {areaIndex} trigger disabled.");
        }
        else
        {
            Debug.Log($"Non-player object {other.name} entered AreaTrigger {areaIndex}");
        }
    }
}
