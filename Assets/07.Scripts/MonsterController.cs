using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public Transform player; // 플레이어의 위치
    public float moveSpeed = 3.0f; // 몬스터의 이동 속도
    public float attackDistance = 5.0f; // 공격 거리
    public float stopDistance = 1.5f; // 플레이어와의 최종 정지 거리
    public float attackRate = 1.0f; // 공격 속도
    private float nextAttackTime = 0f; // 다음 공격 시간

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어에게 이동
        MoveToPlayer();

        // 공격 조건 확인
        if (distanceToPlayer <= stopDistance)
        {
            Attack();
        }
    }

    void MoveToPlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = moveSpeed;
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // 공격 로직 구현 (애니메이션, 데미지 처리 등)
            Debug.Log("Attack the player!");

            nextAttackTime = Time.time + attackRate;
        }
    }
}