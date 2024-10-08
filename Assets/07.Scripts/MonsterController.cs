using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 3.0f; // 몬스터의 이동 속도
    public float attackDistance = 5.0f; // 공격 거리
    public float stopDistance = 1.5f; // 플레이어와의 최종 정지 거리
    public float attackRate = 1.0f; // 공격 속도
    private float nextAttackTime = 0f; // 다음 공격 시간

    public float flockingRadius = 5.0f; // 플로킹을 위한 반경
    public float separationDistance = 1.5f; // 몬스터들 간의 거리
    private NavMeshAgent agent;
    public Transform player; // 플레이어의 Transform
    private FlockingManager flockingManager; // FlockingManager 참조

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // "Player" 태그를 가진 오브젝트 찾기
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // FlockingManager 찾기
        flockingManager = FindObjectOfType<FlockingManager>();
        if (flockingManager != null)
        {
            flockingManager.RegisterMonster(this); // FlockingManager에 등록
        }

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
        }
    }

    void OnDestroy()
    {
        if (flockingManager != null)
        {
            flockingManager.UnregisterMonster(this); // 제거될 때 FlockingManager에서 등록 해제
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null! Make sure the player is set.");
            return; // 플레이어가 없으면 업데이트 중지
        }

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 몬스터가 플레이어에게 이동
        MoveToPlayer();

        // 공격 조건 확인
        if (distanceToPlayer <= stopDistance)
        {
            Attack();
        }
    }

    void MoveToPlayer()
    {
        if (agent != null && player != null)
        {
            // 플로킹 동작 추가
            Vector3 flockingForce = FlockingBehavior();
            Vector3 targetPosition = player.position + flockingForce;

            agent.SetDestination(targetPosition);
            agent.speed = moveSpeed;
        }
        else
        {
            Debug.LogError("Agent or player is null for " + gameObject.name);
        }
    }

    public Vector3 FlockingBehavior() // 접근 제한을 public으로 변경
    {
        Vector3 separationForce = CalculateSeparation();
        Vector3 alignmentForce = CalculateAlignment();
        Vector3 cohesionForce = CalculateCohesion();

        // 힘들을 더하고 반환
        return separationForce + alignmentForce + cohesionForce;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 force = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(transform.position, flockingRadius);

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor != this.GetComponent<Collider>()) // 자기 자신 제외
            {
                float distance = Vector3.Distance(transform.position, neighbor.transform.position);
                if (distance < separationDistance)
                {
                    // 몬스터들 간의 분리 힘 계산
                    Vector3 direction = (transform.position - neighbor.transform.position).normalized;
                    force += direction / distance; // 가까울수록 힘이 강해지도록
                }
            }
        }
        return force;
    }

    Vector3 CalculateAlignment()
    {
        // 주변 몬스터 찾기
        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, flockingRadius);

        Vector3 alignment = Vector3.zero;
        int count = 0;

        foreach (Collider monster in nearbyMonsters)
        {
            if (monster.transform != transform) // 자신을 제외
            {
                MonsterController otherMonster = monster.GetComponent<MonsterController>();
                if (otherMonster != null) // null 체크
                {
                    alignment += otherMonster.agent.velocity; // 다른 몬스터의 속도 추가
                    count++;
                }
            }
        }

        if (count > 0)
        {
            alignment /= count; // 평균 속도 계산
            alignment = alignment.normalized * moveSpeed; // 속도를 설정
            alignment -= agent.velocity; // 현재 속도와의 차이
        }

        return alignment;
    }

    Vector3 CalculateCohesion()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        Collider[] neighbors = Physics.OverlapSphere(transform.position, flockingRadius);
        foreach (Collider neighbor in neighbors)
        {
            if (neighbor != this.GetComponent<Collider>())
            {
                centerOfMass += neighbor.transform.position; // 주변 몬스터의 중심 계산
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return (centerOfMass - transform.position).normalized; // 응집 힘 반환
        }
        return Vector3.zero;
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
