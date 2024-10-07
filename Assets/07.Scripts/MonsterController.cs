using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public Transform player; // �÷��̾��� ��ġ
    public float moveSpeed = 3.0f; // ������ �̵� �ӵ�
    public float attackDistance = 5.0f; // ���� �Ÿ�
    public float stopDistance = 1.5f; // �÷��̾���� ���� ���� �Ÿ�
    public float attackRate = 1.0f; // ���� �ӵ�
    private float nextAttackTime = 0f; // ���� ���� �ð�

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // �÷��̾���� �Ÿ� ���
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // �÷��̾�� �̵�
        MoveToPlayer();

        // ���� ���� Ȯ��
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
            // ���� ���� ���� (�ִϸ��̼�, ������ ó�� ��)
            Debug.Log("Attack the player!");

            nextAttackTime = Time.time + attackRate;
        }
    }
}