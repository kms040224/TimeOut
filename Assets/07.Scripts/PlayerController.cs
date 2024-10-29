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
    public float rollSpeed = 15f; // 굴러가는 속도
    public float rollDistance = 3f; // 굴러가는 거리
    public float rollCooldown = 5f; // 굴러가는 쿨타임 설정
    private float lastRollTime = -5f; // 굴러가기 쿨타임 초기화
    public AniController aniController;

    private Vector3 destinationPoint;
    private bool shouldMove = false;
    private bool isUsingFlamethrower = false;
    private bool isInvincible = false; // 무적 상태 여부
    public float knockbackForce = 5f;  // 넉백 힘
    private float flamethrowerCooldownTimer = 0f;
    private Renderer playerRenderer;

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
        playerRenderer = GetComponent<Renderer>();
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
                animator.SetBool("isMoving", false); // 멈췄을 때 애니메이션 중지
            }
            else
            {
                animator.SetBool("isMoving", true); // 이동 중 애니메이션 재생
            }

        }
        else
        {
            animator.SetBool("isMoving", false);
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
            AimAtCursor();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LaunchMeteor();
            AimAtCursor();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastRollTime >= rollCooldown) // 쿨타임 체크
            {
                AimAtCursor();
                StartCoroutine(RollTowardsCursor());
            }
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

        animator.SetTrigger("FireThrower");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject flamethrower = Instantiate(flamethrowerPrefab, flamethrowerSpawnPoint.position, Quaternion.identity);
            flamethrower.transform.LookAt(hit.point);

            yield return new WaitForSeconds(flamethrowerDuration);
        }

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

        // 애니메이션 트리거 추가
        animator.SetTrigger("FireFlooring");

        // 장판 생성
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 장판 생성
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

        animator.SetTrigger("Meteor");
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

    IEnumerator RollTowardsCursor()
    {
        // 쿨타임이 지나지 않았으면 실행하지 않음
        if (Time.time - lastRollTime < rollCooldown)
        {
            Debug.Log("구르기 스킬이 아직 쿨타임 중입니다.");
            yield break; // Coroutine 종료
        }

        lastRollTime = Time.time; // 마지막 구르기 시간 갱신

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 rollDirection = (hit.point - transform.position).normalized;
            rollDirection.y = 0;

            // 캐릭터 굴러가는 애니메이션 트리거
            animator.SetTrigger("Roll");

            Vector3 startPosition = transform.position; // 시작 위치
            Vector3 rollTargetPosition = transform.position + rollDirection * rollDistance; // 목표 위치
            float elapsedTime = 0f; // 경과 시간
            float rollDuration = rollDistance / rollSpeed; // 구르기 시간

            while (elapsedTime < rollDuration)
            {
                // 경과 시간 업데이트
                elapsedTime += Time.deltaTime;
                // 현재 위치를 보간하여 이동
                transform.position = Vector3.Lerp(startPosition, rollTargetPosition, elapsedTime / rollDuration);
                yield return null; // 다음 프레임까지 대기
            }

            // 최종 위치 설정
            transform.position = rollTargetPosition;
            Debug.Log("캐릭터가 굴러서 이동: " + rollTargetPosition);
        }
    }

    public void TakeDamage(Vector3 hitDirection, float damage)
    {
        animator.SetTrigger("CharacterHit");
        if (!isInvincible)
        {
            StartCoroutine(Knockback(hitDirection));
            StartCoroutine(InvincibilityCoroutine());
        }
            PlayerHealthManager.Instance.TakeDamage(damage);
        UpdateHealthBar();
        StartCoroutine(InvincibilityAndBlinking());
    }
    private IEnumerator Knockback(Vector3 hitDirection)
    {
        Vector3 knockbackDir = hitDirection.normalized;
        float knockbackTime = 0.1f;  // 넉백 지속 시간

        while (knockbackTime > 0)
        {
            transform.position += knockbackDir * knockbackForce * Time.deltaTime;
            knockbackTime -= Time.deltaTime;
            yield return null;
        }
    }

    // 무적 상태를 관리하는 코루틴
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;  // 무적 상태 활성화
        yield return new WaitForSeconds(0.5f);
        isInvincible = false; // 무적 상태 해제
    }
    private IEnumerator InvincibilityAndBlinking()
    {
        isInvincible = true;

        // 0.5초 동안 깜빡임 효과
        float blinkDuration = 0.5f;
        float blinkInterval = 0.1f; // 깜빡임 간격
        float endTime = Time.time + blinkDuration;

        while (Time.time < endTime)
        {
            // Renderer 활성화/비활성화로 깜빡임 구현
            playerRenderer.enabled = !playerRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 깜빡임 종료 후 Renderer 활성화
        playerRenderer.enabled = true;
        isInvincible = false;
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Debug.Log("Player died!");
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
        }
    }
}