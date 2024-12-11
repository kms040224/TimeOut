using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArcherController : MonsterController
{
    public GameObject arrowPrefab; // ȭ�� ������
    public Transform shootPoint; // ȭ�� �߻� ��ġ
    public float arrowSpeed = 10f; // ȭ�� �ӵ�

    protected override void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // �÷��̾���� �Ÿ��� stopDistance ������ ���� ����
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= stopDistance)  // stopDistance�� ���� �Ÿ��� �����Ǿ�� ��
            {
                Debug.Log("Goblin Archer shoots an arrow!");

                nextAttackTime = Time.time + attackRate;

                // ȭ�� �߻�
                if (arrowPrefab != null && shootPoint != null)
                {
                    GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
                    Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

                    if (arrowRb != null)
                    {
                        // �÷��̾� �������� ȭ�� �߻�
                        Vector3 direction = (player.position - shootPoint.position).normalized;
                        arrowRb.velocity = direction * arrowSpeed;
                    }
                }

                // ���� �ִϸ��̼� Ʈ����
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
    }
}