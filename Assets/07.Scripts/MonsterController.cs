using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public float moveSpeed = 3.0f;
    public float attackDistance = 5.0f;
    public float stopDistance = 1.5f;
    public float attackRate = 1.0f;
    protected float nextAttackTime = 0f;
    public float knockbackForce = 5.0f;
    public float knockbackDuration = 0.5f;
    private bool isKnockedBack = false;

    public float flockingRadius = 5.0f;
    public float separationDistance = 1.5f;
    protected NavMeshAgent agent;
    public Transform player;
    private FlockingManager flockingManager;
    private PlayerController playerController;
    private GameManager gameManager;
    protected Animator animator;

    public int health = 100;

    // 드랍할 아이템 리스트
    public List<GameObject> dropItems;
    public float dropChance = 0.5f; // 아이템 드랍 확률 (50%)

    private bool isDead = false; // 죽었는지 확인하는 변수

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
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
        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        if (flockingManager != null)
        {
            flockingManager.UnregisterMonster(this);
        }
    }

    protected virtual void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player reference is null! Make sure the player is set.");
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 플레이어와의 거리가 stopDistance보다 멀면 이동
        if (distanceToPlayer > stopDistance)
        {
            MoveToPlayer();
        }
        else
        {
            agent.ResetPath(); // 이동 멈춤

            // 플레이어 쪽을 바라봄
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
            }
        }

        // 공격 거리가 유효하면 공격
        if (distanceToPlayer <= attackDistance)
        {
            Attack();
        }
    }

    void MoveToPlayer()
    {
        if (agent != null && agent.isActiveAndEnabled && player != null)
        {
            Vector3 flockingForce = FlockingBehavior();
            Vector3 targetPosition = player.position + flockingForce;

            // 플레이어와 몬스터 사이에 장애물이 있는지 Raycast로 확인
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, directionToPlayer, out hit, agent.stoppingDistance))
            {
                // 장애물이 있을 경우, 장애물을 회피하는 벡터 계산
                if (hit.collider != null && hit.collider.CompareTag("NavMeshObstacle"))
                {
                    // 장애물 벡터와 수평 방향을 교차시켜 회피 벡터를 구함
                    Vector3 avoidanceDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

                    // 장애물을 피할 새로운 위치 계산
                    Vector3 newTargetPosition = transform.position + avoidanceDirection * 2f; // 2f는 피하는 정도를 설정하는 값
                    targetPosition = newTargetPosition;
                }
            }

            // 경로 재계산 강제 호출 (경로가 유효하지 않으면 재계산)
            if (agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                agent.ResetPath();
                agent.SetDestination(targetPosition);
            }

            // 몬스터가 플레이어 쪽을 바라보도록 회전
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * agent.angularSpeed);
            }

            // 새로운 targetPosition으로 NavMeshAgent 설정
            agent.SetDestination(targetPosition);
            agent.speed = moveSpeed;
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

    protected virtual void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            Debug.Log("Attack the player!");
            PlayerController playerController = player.GetComponent<PlayerController>();
            nextAttackTime = Time.time + attackRate;
            if (playerController != null)
            {
                // 몬스터에서 플레이어 방향 벡터 계산
                Vector3 hitDirection = (player.transform.position - transform.position).normalized;
                int damage = 10; // 예시로 설정한 데미지 값

                // 방향과 데미지를 전달하여 TakeDamage 호출
                playerController.TakeDamage(hitDirection, damage);
            }
            animator.SetTrigger("Attack");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        animator.SetTrigger("TakeDamage");
        Debug.Log("Monster took damage! Current health: " + health);
        DamageTextController.Instance.CreateDamageText(transform.position, damage);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            // 넉백 처리
            StartKnockback();
        }
    }

    private void StartKnockback()
    {
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false; // 비활성화 상태로 시작
        rb.AddForce(-transform.forward * knockbackForce, ForceMode.Impulse); // 반대 방향으로 힘 적용

        // 일정 시간 후에 Rigidbody 제거
        StartCoroutine(RemoveRigidbody(rb));
    }

    private IEnumerator RemoveRigidbody(Rigidbody rb)
    {
        yield return new WaitForSeconds(0.1f); // 넉백 시간이 끝날 때까지 대기
        Destroy(rb);
    }
    private void Die()
    {
        if (isDead) return; // 이미 죽은 경우 실행 안 함

        isDead = true; // 죽은 상태로 설정
        Debug.Log("Monster died!");
        DropItem(); // 아이템 드랍 처리
        Destroy(gameObject);
        gameManager.OnMonsterKilled();
    }

    // 아이템 드랍 메서드
    void DropItem()
    {
        if (dropItems.Count > 0 && Random.value <= dropChance)
        {
            int randomIndex = Random.Range(0, dropItems.Count);
            GameObject droppedItemPrefab = dropItems[randomIndex];

            GameObject droppedItemObject = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);

            // 몬스터와 아이템 충돌 무시
            Collider itemCollider = droppedItemObject.GetComponent<Collider>();
            Collider monsterCollider = GetComponent<Collider>();

            if (itemCollider != null && monsterCollider != null)
            {
                Physics.IgnoreCollision(itemCollider, monsterCollider);
            }

            FieldItems fieldItem = droppedItemObject.GetComponent<FieldItems>();
            Item itemToSet = null;

            if (fieldItem != null)
            {
                itemToSet = ItemDatabase.instance.itemDB.Find(item => item.itemPrefab == droppedItemPrefab);
                fieldItem.SetItem(itemToSet);
            }

            if (itemToSet != null)
            {
                Debug.Log("Dropped item: " + itemToSet.itemName);
            }
        }
    }
}
