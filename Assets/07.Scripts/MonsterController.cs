using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 3.0f; // ������ �̵� �ӵ�
    public float attackDistance = 5.0f; // ���� �Ÿ�
    public float stopDistance = 1.5f; // �÷��̾���� ���� ���� �Ÿ�
    public float attackRate = 1.0f; // ���� �ӵ�
    private float nextAttackTime = 0f; // ���� ���� �ð�

    public float flockingRadius = 5.0f; // �÷�ŷ�� ���� �ݰ�
    public float separationDistance = 1.5f; // ���͵� ���� �Ÿ�
    private NavMeshAgent agent;
    public Transform player; // �÷��̾��� Transform
    private FlockingManager flockingManager; // FlockingManager ����

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // "Player" �±׸� ���� ������Ʈ ã��
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // FlockingManager ã��
        flockingManager = FindObjectOfType<FlockingManager>();
        if (flockingManager != null)
        {
            flockingManager.RegisterMonster(this); // FlockingManager�� ���
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
            flockingManager.UnregisterMonster(this); // ���ŵ� �� FlockingManager���� ��� ����
        }
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null! Make sure the player is set.");
            return; // �÷��̾ ������ ������Ʈ ����
        }

        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���Ͱ� �÷��̾�� �̵�
        MoveToPlayer();

        // ���� ���� Ȯ��
        if (distanceToPlayer <= stopDistance)
        {
            Attack();
        }
    }

    void MoveToPlayer()
    {
        if (agent != null && player != null)
        {
            // �÷�ŷ ���� �߰�
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

    public Vector3 FlockingBehavior() // ���� ������ public���� ����
    {
        Vector3 separationForce = CalculateSeparation();
        Vector3 alignmentForce = CalculateAlignment();
        Vector3 cohesionForce = CalculateCohesion();

        // ������ ���ϰ� ��ȯ
        return separationForce + alignmentForce + cohesionForce;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 force = Vector3.zero;
        Collider[] neighbors = Physics.OverlapSphere(transform.position, flockingRadius);

        foreach (Collider neighbor in neighbors)
        {
            if (neighbor != this.GetComponent<Collider>()) // �ڱ� �ڽ� ����
            {
                float distance = Vector3.Distance(transform.position, neighbor.transform.position);
                if (distance < separationDistance)
                {
                    // ���͵� ���� �и� �� ���
                    Vector3 direction = (transform.position - neighbor.transform.position).normalized;
                    force += direction / distance; // �������� ���� ����������
                }
            }
        }
        return force;
    }

    Vector3 CalculateAlignment()
    {
        // �ֺ� ���� ã��
        Collider[] nearbyMonsters = Physics.OverlapSphere(transform.position, flockingRadius);

        Vector3 alignment = Vector3.zero;
        int count = 0;

        foreach (Collider monster in nearbyMonsters)
        {
            if (monster.transform != transform) // �ڽ��� ����
            {
                MonsterController otherMonster = monster.GetComponent<MonsterController>();
                if (otherMonster != null) // null üũ
                {
                    alignment += otherMonster.agent.velocity; // �ٸ� ������ �ӵ� �߰�
                    count++;
                }
            }
        }

        if (count > 0)
        {
            alignment /= count; // ��� �ӵ� ���
            alignment = alignment.normalized * moveSpeed; // �ӵ��� ����
            alignment -= agent.velocity; // ���� �ӵ����� ����
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
                centerOfMass += neighbor.transform.position; // �ֺ� ������ �߽� ���
                count++;
            }
        }

        if (count > 0)
        {
            centerOfMass /= count;
            return (centerOfMass - transform.position).normalized; // ���� �� ��ȯ
        }
        return Vector3.zero;
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            // ���� ���� ���� (�ִϸ��̼�, ������ ó�� ��)
            Debug.Log("Attack the player!");

            nextAttackTime = Time.time + attackRate;
        }
    }
}
