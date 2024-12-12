using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArcherController : MonsterController
{
    public GameObject arrowPrefab; // ȭ�� ������
    public Transform shootPoint; // ȭ�� �߻� ��ġ
    public float arrowSpeed = 10f; // ȭ�� �ӵ�
    public float rotationSpeed = 5f; // ȸ�� �ӵ�

    protected override void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackDistance)
            {
                base.Attack();
            }
            else if (distanceToPlayer <= stopDistance)
            {
                PerformRangedAttack();
            }
        }
    }

    private void PerformRangedAttack()
    {
        if (arrowPrefab == null || shootPoint == null)
        {
            return;
        }

        // ��� ���İ� �÷��̾ �ٶ󺸵��� ȸ��
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // ���� ���� ȸ���� ����
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // ���Ÿ� ���� �ִϸ��̼� ����
        if (animator != null)
        {
            animator.SetTrigger("RangedAttack");
        }

        // ȭ�� �߻�
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

        if (arrowRb != null)
        {
            Vector3 arrowDirection = (player.position - shootPoint.position).normalized;
            arrowRb.velocity = arrowDirection * arrowSpeed;
        }

        // ���� ���� �ð� ����
        nextAttackTime = Time.time + attackRate;
    }

    protected override void Update()
    {
        base.Update();

        if (animator != null)
        {
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("IsWalking", isMoving);
        }

        Attack();
    }
}
