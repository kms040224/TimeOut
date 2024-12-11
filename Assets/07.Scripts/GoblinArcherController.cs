using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArcherController : MonsterController
{
    public GameObject arrowPrefab; // 화살 프리팹
    public Transform shootPoint; // 화살 발사 위치
    public float arrowSpeed = 10f; // 화살 속도

    protected override void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // 플레이어와의 거리가 stopDistance 이하일 때만 공격
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer <= stopDistance)  // stopDistance가 적정 거리로 설정되어야 함
            {
                Debug.Log("Goblin Archer shoots an arrow!");

                nextAttackTime = Time.time + attackRate;

                // 화살 발사
                if (arrowPrefab != null && shootPoint != null)
                {
                    GameObject arrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
                    Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

                    if (arrowRb != null)
                    {
                        // 플레이어 방향으로 화살 발사
                        Vector3 direction = (player.position - shootPoint.position).normalized;
                        arrowRb.velocity = direction * arrowSpeed;
                    }
                }

                // 공격 애니메이션 트리거
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
    }
}