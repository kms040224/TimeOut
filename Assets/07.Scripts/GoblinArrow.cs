using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : MonoBehaviour
{
    public float damage = 10f; // ȭ�� ������

    void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹���� ��
        if (other.CompareTag("Player"))
        {
            // �÷��̾��� TakeDamage �޼��带 ȣ���ؼ� �������� ����
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // ȭ���� �÷��̾�� �浹�ϸ� �浹 ������ ���
                Vector3 hitDirection = (other.transform.position - transform.position).normalized;

                // TakeDamage �޼��� ȣ�� (�浹 ����� ������ ����)
                playerController.TakeDamage(hitDirection, damage);
            }

            // ȭ�� �����
            Destroy(gameObject);
        }
    }
}