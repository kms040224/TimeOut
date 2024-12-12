using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public int damage = 10;
    public LineRenderer lineRenderer;
    private BoxCollider boxCollider;

    private void Awake()
    {
        // BoxCollider�� �������� �߰�
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true; // Ʈ���ŷ� ����
    }

    private void Update()
    {
        SyncColliderWithLineRenderer();
    }

    private void SyncColliderWithLineRenderer()
    {
        if (lineRenderer.positionCount >= 2)
        {
            // LineRenderer�� �������� ������ ��������
            Vector3 startPoint = lineRenderer.GetPosition(0);
            Vector3 endPoint = lineRenderer.GetPosition(1);

            // BoxCollider�� �߰� ���� ���
            Vector3 midPoint = (startPoint + endPoint) / 2; // �߰� ����
            float length = Vector3.Distance(startPoint, endPoint); // ������ ����

            // BoxCollider�� ��ġ�� ȸ�� ���� (�߽��� (0, 0, 0)���� ����)
            boxCollider.transform.position = midPoint;
            boxCollider.transform.rotation = Quaternion.LookRotation(endPoint - startPoint);

            // BoxCollider ũ�� ����
            boxCollider.size = new Vector3(0.1f, 0.1f, length); // 0.1f�� ������ �β�
            boxCollider.center = Vector3.zero;  // BoxCollider�� �߽��� (0, 0, 0)���� ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector3 hitDirection = other.transform.position - transform.position;
                player.TakeDamage(hitDirection, damage); // �÷��̾�� �������� �˹� ����
            }
        }
    }
}