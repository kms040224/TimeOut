using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Camera mainCamera;
    public GameObject magicAttackPrefab;
    public GameObject flamethrowerPrefab;
    public GameObject fireOrbPrefab;
    public GameObject areaEffectPrefab;
    public GameObject meteorPrefab;
    public GameObject GameOverPanel;
    public Transform magicAttackSpawnPoint;
    public Transform flamethrowerSpawnPoint;
    public float movementSpeed = 10f;
    public float rotationSpeed = 10f;
    public float flamethrowerDuration = 1.5f;
    public float flamethrowerCooldown = 12f;
    public float teleportDistance = 1f;
    public float teleportCooldown = 8.0f;
    private float lastTeleportTime = -8.0f;
    public float areaEffectCooldown = 10.0f;
    private float lastAreaEffectTime = -10.0f;
    public float meteorCooldown = 40f;
    private float lastMeteorTime = -40f;
    public float rollSpeed = 15f;
    public float rollDistance = 3f;
    public float rollCooldown = 5f;
    private float lastRollTime = -5f;
    public AniController aniController;
    public PlayerStats playerStats;

    private Vector3 destinationPoint;
    public bool shouldMove = false;
    private bool isUsingFlamethrower = false;
    private bool isInvincible = false;
    public float knockbackForce = 5f;
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

        if (playerRenderer == null)
        {
            playerRenderer = GetComponentInChildren<Renderer>();
        }
    }

    void Update()
    {
        if (PlayerHealthManager.Instance.health <= 0)
        {
            Die();
            return;
        }

        // Flamethrower 사용 시
        if (Input.GetKeyDown(KeyCode.Q) && !isUsingFlamethrower && flamethrowerCooldownTimer <= 0)
        {
            AimAtCursor();
            StartCoroutine(UseFlamethrower());
        }

        if (flamethrowerCooldownTimer > 0)
        {
            flamethrowerCooldownTimer -= Time.deltaTime;
        }

        // 마우스 우클릭으로 이동
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
            if (Vector3.Distance(transform.position, destinationPoint) < 0.1f)
            {
                shouldMove = false; // 도착하면 이동 중지
                animator.SetBool("isMoving", false); // 애니메이션 중지
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(destinationPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerStats.rotationSpeed * Time.deltaTime); // 스크립터블 오브젝트의 rotationSpeed 사용

                transform.position = Vector3.MoveTowards(transform.position, destinationPoint, playerStats.movementSpeed * Time.deltaTime); // 스크립터블 오브젝트의 movementSpeed 사용
                animator.SetBool("isMoving", true); // 이동 중 애니메이션 재생
            }
        }
        else
        {
            animator.SetBool("isMoving", false); // 이동하지 않을 때 애니메이션 중지
        }

        if (Input.GetKeyDown(KeyCode.A) && !isUsingFlamethrower)
        {
            AimAtCursor();
            ShootMagicAttack();
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
            if (Time.time - lastRollTime >= playerStats.rollCooldown) // 쿨타임 체크
            {
                AimAtCursor();
                StartCoroutine(RollTowardsCursor());
            }
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

            yield return new WaitForSeconds(playerStats.flamethrowerDuration); // 스크립터블 오브젝트의 flamethrowerDuration 사용
        }

        flamethrowerCooldownTimer = playerStats.flamethrowerCooldown; // 스크립터블 오브젝트의 flamethrowerCooldown 사용
        isUsingFlamethrower = false;
    }

    // MagicAttack 발사
    void ShootMagicAttack()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject magicAttack = ObjectPool.Instance.GetMagicAttack(); // MagicAttack을 풀에서 가져오기
            magicAttack.transform.position = magicAttackSpawnPoint.position;
            magicAttack.transform.rotation = Quaternion.identity;

            MagicAttackController magicAttackController = magicAttack.GetComponent<MagicAttackController>();
            if (magicAttackController != null)
            {
                magicAttackController.Launch(hit.point);
            }

            Debug.Log("Magic Attack shot towards: " + hit.point);
        }
    }

    void TeleportAndSpawnFireOrbs()
    {
        if (Time.time - lastTeleportTime < playerStats.teleportCooldown) // 스크립터블 오브젝트의 teleportCooldown 사용
        {
            Debug.Log("스킬이 아직 쿨타임 중입니다.");
            return;
        }

        lastTeleportTime = Time.time;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 teleportDirection = (hit.point - transform.position).normalized;
            teleportDirection.y = 0;
            transform.position += teleportDirection * playerStats.teleportDistance; // 스크립터블 오브젝트의 teleportDistance 사용

            SpawnFireOrbs();
        }
    }

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

    void LayDownAreaEffect()
    {
        if (Time.time - lastAreaEffectTime < playerStats.areaEffectCooldown) // 스크립터블 오브젝트의 areaEffectCooldown 사용
        {
            Debug.Log("장판 쿨타임 중입니다.");
            return;
        }

        animator.SetTrigger("FireFlooring");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Instantiate(areaEffectPrefab, hit.point, Quaternion.identity);
            lastAreaEffectTime = Time.time;
        }
    }

    void LaunchMeteor()
    {
        if (Time.time - lastMeteorTime < playerStats.meteorCooldown) // 스크립터블 오브젝트의 meteorCooldown 사용
        {
            Debug.Log("메테오 쿨타임 중입니다.");
            return;
        }

        animator.SetTrigger("FireMeteor");

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 메테오를 소환하고, MeteorController의 Launch 메서드를 호출하여 마우스 방향으로 발사
            GameObject meteor = Instantiate(meteorPrefab, transform.position, Quaternion.identity);
            MeteorController meteorController = meteor.GetComponent<MeteorController>();

            if (meteorController != null)
            {
                meteorController.Launch(hit.point); // 마우스 클릭 위치를 목표로 설정
            }

            lastMeteorTime = Time.time;
        }
    }

    IEnumerator RollTowardsCursor()
    {
        lastRollTime = Time.time;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 rollDirection = (hit.point - transform.position).normalized;
            rollDirection.y = 0;

            float rollDistance = playerStats.rollDistance; // 스크립터블 오브젝트의 rollDistance 사용
            Vector3 startPosition = transform.position;
            Vector3 endPosition = startPosition + rollDirection * rollDistance;

            float rollTime = playerStats.rollTime; // 스크립터블 오브젝트의 rollTime 사용
            float elapsedTime = 0f;

            while (elapsedTime < rollTime)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / rollTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = endPosition;
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
        StartCoroutine(InvincibilityAndBlinking());
    }
    private IEnumerator Knockback(Vector3 hitDirection)
    {
        Vector3 knockbackDir = hitDirection.normalized;
        knockbackDir.y = 0;
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