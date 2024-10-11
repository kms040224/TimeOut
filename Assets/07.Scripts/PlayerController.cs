using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Camera mainCamera;
    public GameObject fireballPrefab;
    public GameObject flamethrowerPrefab;
    public GameObject fireOrbPrefab;  // 화염구체 프리팹 추가
    public GameObject areaEffectPrefab; // 장판 프리팹 추가
    public GameObject meteorPrefab;
    public GameObject GameOverPanel;
    public Transform fireballSpawnPoint;
    public Transform flamethrowerSpawnPoint;
    public float movementSpeed = 10f;
    public float rotationSpeed = 10f;
    public float flamethrowerDuration = 1.5f;
    public float flamethrowerCooldown = 12f;
    public Slider healthSlider;
    public float teleportDistance = 1f; // 순간이동 거리 설정
    public float teleportCooldown = 8.0f;
    private float lastTeleportTime = -8.0f;
    public float areaEffectCooldown = 10.0f; // 장판 쿨타임 설정
    private float lastAreaEffectTime = -10.0f; // 장판 쿨타임 초기화
    public float meteorCooldown = 40f; // 메테오 쿨타임 설정
    private float lastMeteorTime = -40f; // 메테오 쿨타임 초기화

    private Vector3 destinationPoint;
    private bool shouldMove = false;
    private bool isUsingFlamethrower = false;
    private float flamethrowerCooldownTimer = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);

        if (PlayerHealthManager.Instance != null)
        {
            UpdateHealthBar(); // 체력 바 업데이트
        }
        else
        {
            Debug.LogError("PlayerHealthManager 인스턴스가 null입니다. PlayerHealthManager가 씬에 추가되어 있는지 확인하세요.");
        }

        if (healthSlider == null)
        {
            Debug.LogError("Health Slider가 할당되지 않았습니다.");
        }
    }

    void Update()
    {
        if (PlayerHealthManager.Instance != null)
        {
            UpdateHealthBar();
        }

        if (PlayerHealthManager.Instance.health <= 0)
        {
            Die();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isUsingFlamethrower && flamethrowerCooldownTimer <= 0)
        {
            AimAtCursor();
            StartCoroutine(UseFlamethrower());
        }

        if (flamethrowerCooldownTimer > 0)
        {
            flamethrowerCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(1) && !isUsingFlamethrower)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    destinationPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    shouldMove = true;
                }
            }
        }

        if (shouldMove && !isUsingFlamethrower)
        {
            Quaternion targetRotation = Quaternion.LookRotation(destinationPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, destinationPoint, movementSpeed * Time.deltaTime);

            if (transform.position == destinationPoint)
            {
                shouldMove = false;
            }

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (Input.GetKeyDown(KeyCode.A) && !isUsingFlamethrower)
        {
            AimAtCursor();
            ShootFireball();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            TeleportAndSpawnFireOrbs();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            LayDownAreaEffect();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LaunchMeteor();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)PlayerHealthManager.Instance.CurrentHealth / PlayerHealthManager.Instance.maxHealth;
        }
        else
        {
            Debug.LogError("Health Slider가 null입니다.");
        }
    }

    private void AimAtCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 direction = (hit.point - transform.position).normalized;
            direction.y = 0;

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }
    }

    private IEnumerator UseFlamethrower()
    {
        isUsingFlamethrower = true;

        animator.SetBool("isUsingFlamethrower", true);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject flamethrower = Instantiate(flamethrowerPrefab, flamethrowerSpawnPoint.position, Quaternion.identity);
            flamethrower.transform.LookAt(hit.point);

            yield return new WaitForSeconds(flamethrowerDuration);
        }

        animator.SetBool("isUsingFlamethrower", false);
        flamethrowerCooldownTimer = flamethrowerCooldown;
        isUsingFlamethrower = false;
    }

    void ShootFireball()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);
            FireballController fireballController = fireball.GetComponent<FireballController>();

            if (fireballController != null)
            {
                fireballController.Launch(hit.point);
            }

            Debug.Log("Fireball shot towards: " + hit.point);
        }
    }

    // 마우스 방향으로 캐릭터를 1만큼 순간이동하고 화염구체를 생성하는 메서드
    void TeleportAndSpawnFireOrbs()
    {
        // 쿨타임이 지나지 않았으면 실행하지 않음
        if (Time.time - lastTeleportTime < teleportCooldown)
        {
            Debug.Log("스킬이 아직 쿨타임 중입니다.");
            return;
        }

        // 순간이동할 수 있는 경우, 현재 시간을 기록
        lastTeleportTime = Time.time;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 teleportDirection = (hit.point - transform.position).normalized;
            teleportDirection.y = 0;
            transform.position += teleportDirection * teleportDistance;

            // 캐릭터 주위에 두 개의 화염구체 생성
            SpawnFireOrbs();
        }
    }

    // 화염구체를 생성하고 가장 가까운 몬스터를 향해 날아가게 하는 메서드
    void SpawnFireOrbs()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject fireOrb = Instantiate(fireOrbPrefab, transform.position + (Vector3.right * (i == 0 ? 1 : -1)), Quaternion.identity);
            FireOrbController fireOrbController = fireOrb.GetComponent<FireOrbController>();

            if (fireOrbController != null)
            {
                GameObject nearestMonster = FindNearestMonster();
                if (nearestMonster != null)
                {
                    fireOrbController.SetTarget(nearestMonster.transform);
                }
            }
        }
    }

    // 가장 가까운 몬스터를 찾는 메서드
    GameObject FindNearestMonster()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        GameObject nearestMonster = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject monster in monsters)
        {
            float distance = Vector3.Distance(transform.position, monster.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestMonster = monster;
            }
        }

        return nearestMonster;
    }

    // 장판을 깔고 쿨타임을 관리하는 메서드
    void LayDownAreaEffect()
    {
        if (Time.time - lastAreaEffectTime < areaEffectCooldown)
        {
            Debug.Log("장판 쿨타임 중입니다.");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 장판 생성
            Vector3 areaEffectPosition = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
            Instantiate(areaEffectPrefab, hit.point, Quaternion.identity);
            lastAreaEffectTime = Time.time; // 현재 시간을 기록하여 쿨타임 관리
        }
    }

    void LaunchMeteor()
    {
        // 쿨타임이 지나지 않았으면 실행하지 않음
        if (Time.time - lastMeteorTime < meteorCooldown)
        {
            Debug.Log("메테오 쿨타임 중입니다.");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 메테오 생성
            GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);
            MeteorController meteorController = meteor.GetComponent<MeteorController>();

            if (meteorController != null)
            {
                meteorController.Launch(hit.point); // 메테오 발사
            }

            lastMeteorTime = Time.time; // 현재 시간을 기록하여 쿨타임 관리
        }
    }

    public void TakeDamage(int damage)
    {
        PlayerHealthManager.Instance.TakeDamage(damage);
        UpdateHealthBar();
    }

    private void Die()
    {
        Debug.Log("Player died!");
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
        }
    }
}
