using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearObstacle : MonoBehaviour
{
    [SerializeField] private float riseHeight = 2.0f; // â�� �ö� ����
    [SerializeField] private float riseSpeed = 5.0f;  // â�� �ö󰡴� �ӵ�
    [SerializeField] private float fallSpeed = 2.0f;  // â�� �������� �ӵ�
    [SerializeField] private float interval = 5.0f;   // â�� �ö���� ����
    [SerializeField] private int damage = 10;         // �÷��̾�� �� ������

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.up * riseHeight;
        StartCoroutine(ActivateSpear());
    }

    private IEnumerator ActivateSpear()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // â �ø��� (������)
            yield return MoveToPosition(targetPosition, riseSpeed);

            // ��� ��� (������ ���� ����)
            yield return new WaitForSeconds(0.5f);

            // â ������ (������)
            yield return MoveToPosition(initialPosition, fallSpeed);
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Vector3 hitDirection = (player.transform.position - transform.position).normalized; // â���� �÷��̾� ���� ���
            float damage = 5f; // �� ������ ����
            player.TakeDamage(hitDirection, damage); // TakeDamage �޼��� ȣ��
        }
    }
}