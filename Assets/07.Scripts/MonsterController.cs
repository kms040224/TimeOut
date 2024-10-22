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
    private float nextAttackTime = 0f;

    public float flockingRadius = 5.0f;
    public float separationDistance = 1.5f;
    private NavMeshAgent agent;
    public Transform player;
    private FlockingManager flockingManager;
    private PlayerController playerController;
    private GameManager gameManager;

    public int health = 100;

    // 드랍할 아이템 리스트
    public List<GameObject> dropItems;
    public float dropChance = 0.5f; // 아이템 드랍 확률 (50%)

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
                playerController.TakeDamage(10);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Monster took damage! Current health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
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
            GameObject droppedItemPrefab = dropItems[randomIndex]; // dropItems에서 아이템 프리팹 가져오기

            // 드롭된 아이템 오브젝트 생성
            GameObject droppedItemObject = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
            FieldItems fieldItem = droppedItemObject.GetComponent<FieldItems>();

            Item itemToSet = null; // itemToSet 변수를 여기에 선언

            if (fieldItem != null)
            {
                itemToSet = ItemDatabase.instance.itemDB.Find(item => item.itemPrefab == droppedItemPrefab);
                fieldItem.SetItem(itemToSet); // 드롭된 아이템 설정
            }

            if (itemToSet != null)
            {
                Debug.Log("Dropped item: " + itemToSet.itemName);
            }
        }
    }
}
