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
    private PlayerController playerController;

    public int health = 100; // 몬스터의 체력

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        flockingManager = FindObjectOfType<FlockingManager>();

        if (flockingManager != null)
        {
            flockingManager.RegisterMonster(this);
        }

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
        }
        playerController = player.GetComponent<PlayerController>();
    }

    void OnDestroy()
    {
        if (flockingManager != null)
        {
            flockingManager.UnregisterMonster(this);
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null! Make sure the player is set.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        MoveToPlayer();

        if (distanceToPlayer <= stopDistance)
        {
            Attack();
        }
    }

    void MoveToPlayer()
    {
        if (agent != null && player != null)
        {
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

    public Vector3 FlockingBehavior()
    {
        Vector3 separationForce = CalculateSeparation();
        Vector3 alignmentForce = CalculateAlignment();
        Vector3 cohesionForce = CalculateCohesion();

        return separationForce + alignmentForce + cohesionForce;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 force = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(transform.position, flockingRadius);

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor != this.GetComponent<Collider>())
            {
                float distance = Vector3.Distance(transform.position, neighbor.transform.position);
                if (distance < separationDistance)
                {
                    Vector3 direction = (transform.position - neighbor.transform.position).normalized;
                    force += direction / distance;
                }
            }
        }
        return force;
    }

    Vector3 CalculateAlignment()
    {
        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, flockingRadius);
        Vector3 alignment = Vector3.zero;
        int count = 0;

        foreach (Collider monster in nearbyMonsters)
        {
            if (monster.transform != transform)
            {
                MonsterController otherMonster = monster.GetComponent<MonsterController>();
                if (otherMonster != null)
                {
                    alignment += otherMonster.agent.velocity;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            alignment /= count;
            alignment = alignment.normalized * moveSpeed;
            alignment -= agent.velocity;
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
                centerOfMass += neighbor.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return (centerOfMass - transform.position).normalized;
        }
        return Vector3.zero;
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            Debug.Log("Attack the player!");
            PlayerController playerController = player.GetComponent<PlayerController>();
            nextAttackTime = Time.time + attackRate;
            if (playerController != null)
            {
                playerController.TakeDamage(10); // 플레이어에게 10의 데미지를 입힘
            }
        }
    }

    // 몬스터가 피해를 입는 메서드
    public void TakeDamage(int damage)
    {
        health -= damage; // 체력 감소
        Debug.Log("Monster took damage! Current health: " + health);

        // 체력이 0 이하가 되면 몬스터 사망 처리
        if (health <= 0)
        {
            Die();
        }
    }

    // 몬스터 사망 처리 메서드
    private void Die()
    {
        Debug.Log("Monster died!");
        Destroy(gameObject); // 몬스터 오브젝트 삭제
    }
}
