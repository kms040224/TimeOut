using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArcherController : MonsterController
{
    public GameObject arrowPrefab; // 화살 프리팹
    public Transform shootPoint; // 화살 발사 위치
    public float arrowSpeed = 10f; // 화살 속도
    public float rotationSpeed = 5f; // 회전 속도

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

        // 고블린 아쳐가 플레이어를 바라보도록 회전
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0; // 수직 방향 회전은 무시
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        // 원거리 공격 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("RangedAttack");
        }

        // 화살 발사
        GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

        if (arrowRb != null)
        {
            Vector3 arrowDirection = (player.position - shootPoint.position).normalized;
            arrowRb.velocity = arrowDirection * arrowSpeed;
        }

        // 다음 공격 시간 설정
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
