using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Animator animator;
    public Camera mainCamera;
    public GameObject magicAttackPrefab;
    public GameObject flamethrowerPrefab;
    private bool hasBarrier = false; 
    private float barrierTimer = 0f;
    private bool isBarrierOnCooldown = false; 
    private GameObject activeBarrierEffect;
    public GameObject barrierEffectPrefab;
    public GameObject areaEffectPrefab;
    public GameObject meteorPrefab;
    public GameObject GameOverPanel;
    public Transform magicAttackSpawnPoint;
    public Transform flamethrowerSpawnPoint;
    public float movementSpeed = 10f;
    public float rotationSpeed = 10f;
    public float flamethrowerDuration = 1.5f;
    public float flamethrowerCooldown = 12f;
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
    public AudioClip magicAttackSound;
    public AudioClip walkingSound;
    public AudioClip AttackedSound;
    public AudioClip ActivateBarrierSound;
    public AudioClip DeactivateBarrierSound;
    public AudioClip FlamethrowerSound;
    public AudioClip LayDownAreaSound;
    public AudioClip MeteorSound;
    public AudioClip DeathSound;
    private AudioSource audioSource;

    public GameObject shockwaveEffectPrefab;
    public Transform spawnPoint;
    public float shockwaveDuration = 0.5f;

    private Vector3 destinationPoint;
    public bool shouldMove = false;
    private bool isUsingFlamethrower = false;
    private bool isInvincible = false;
    public float knockbackForce = 5f;
    private float flamethrowerCooldownTimer = 0f;
    private float lastBarrierActivationTime;
    private Renderer playerRenderer;

    public Image qSkillCooldownImage;
    public Image wSkillCooldownImage;
    public Image eSkillCooldownImage;
    public Image rSkillCooldownImage;

    public Text qSkillCooldownText;
    public Text wSkillCooldownText;
    public Text eSkillCooldownText;
    public Text rSkillCooldownText;

    public enum AttributeType
    {
        Fire,
        Lightning
    }
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
        lastBarrierActivationTime = Time.time;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (PlayerHealthManager.Instance.health <= 0)
        {
            Die();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isUsingFlamethrower && flamethrowerCooldownTimer <= 0)
        {
            if (PlayerAttribute.Instance.SelectedAttribute == AttributeType.Fire)
            {
                AimAtCursor();
                StartCoroutine(UseFlamethrower());
            }
            else if (PlayerAttribute.Instance.SelectedAttribute == AttributeType.Lightning)
            {
                AimAtCursor();
                StartCoroutine(UseShockwave());
            }
        }

        if (flamethrowerCooldownTimer > 0)
        {
            flamethrowerCooldownTimer -= Time.deltaTime;
        }

        if (hasBarrier)
        {
            barrierTimer -= Time.deltaTime;
            if (barrierTimer <= 0)
            {
                DeactivateBarrier();
            }
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
                StopWalkingSound(); // 걷는 소리 멈추기
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(destinationPoint - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, playerStats.rotationSpeed * Time.deltaTime); // 스크립터블 오브젝트의 rotationSpeed 사용

                transform.position = Vector3.MoveTowards(transform.position, destinationPoint, playerStats.movementSpeed * Time.deltaTime); // 스크립터블 오브젝트의 movementSpeed 사용
                animator.SetBool("isMoving", true); // 이동 중 애니메이션 재생
                PlayWalkingSound(); // 걷는 소리 재생
            }
        }
        else
        {
            animator.SetBool("isMoving", false); // 이동하지 않을 때 애니메이션 중지
            StopWalkingSound(); // 걷는 소리 멈추기
        }

        if (Input.GetKeyDown(KeyCode.A) && !isUsingFlamethrower)
        {
            AimAtCursor();
            ShootMagicAttack(); // 이 스킬도 속성 기반으로 제어 가능
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (PlayerAttribute.Instance.SelectedAttribute == AttributeType.Fire)
            {
                TryActivateBarrier();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (PlayerAttribute.Instance.SelectedAttribute == AttributeType.Fire)
            {
                LayDownAreaEffect();
                AimAtCursor();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (PlayerAttribute.Instance.SelectedAttribute == AttributeType.Fire)
            {
                LaunchMeteor();
                AimAtCursor();
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.time - lastRollTime >= playerStats.rollCooldown) // 쿨타임 체크
            {
                AimAtCursor();
                StartCoroutine(RollTowardsCursor());
            }
        }

        UpdateCooldownUI(flamethrowerCooldownTimer, playerStats.flamethrowerCooldown, qSkillCooldownImage, qSkillCooldownText);
        UpdateCooldownUI(isBarrierOnCooldown ? playerStats.barrierCooldown - (Time.time - lastBarrierActivationTime) : 0, playerStats.barrierCooldown, wSkillCooldownImage,wSkillCooldownText);
        UpdateCooldownUI(playerStats.areaEffectCooldown - (Time.time - lastAreaEffectTime), playerStats.areaEffectCooldown, eSkillCooldownImage, eSkillCooldownText);
        UpdateCooldownUI(playerStats.meteorCooldown - (Time.time - lastMeteorTime), playerStats.meteorCooldown, rSkillCooldownImage, rSkillCooldownText);

        void UpdateCooldownUI(float currentCooldown, float maxCooldown, Image cooldownImage, Text cooldownText)
        {
            if (currentCooldown > 0)
            {
                cooldownImage.fillAmount = currentCooldown / maxCooldown;
                cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
                cooldownImage.enabled = true;
                cooldownText.enabled = true;
            }
            else
            {
                cooldownImage.fillAmount = 0;
                cooldownText.text = "";
                cooldownImage.enabled = false;
                cooldownText.enabled = false;
            }
        }
    }

    private void PlayWalkingSound()
    {
        if (!audioSource.isPlaying && walkingSound != null)
        {
            audioSource.loop = true; // 반복 재생
            audioSource.clip = walkingSound;
            audioSource.Play();
        }
    }

    private void StopWalkingSound()
    {
        if (audioSource.isPlaying && walkingSound != null)
        {
            audioSource.Stop();
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
        SoundManager.Instance.PlaySound(FlamethrowerSound);
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

            // SoundManager를 사용하여 사운드 재생
            SoundManager.Instance.PlaySound(magicAttackSound);
        }
    }

    void TryActivateBarrier()
    {
        if (hasBarrier)
        {
            Debug.Log("배리어가 이미 활성화 중입니다.");
            return;
        }

        if (isBarrierOnCooldown)
        {
            Debug.Log("배리어 스킬이 쿨타임입니다.");
            return;
        }

        lastBarrierActivationTime = Time.time; // 쿨타임 시작 시간 기록
        ActivateBarrier();
    }

    void ActivateBarrier()
    {
        hasBarrier = true;
        barrierTimer = playerStats.barrierDuration;

        activeBarrierEffect = Instantiate(barrierEffectPrefab, transform.position, Quaternion.identity);
        activeBarrierEffect.transform.parent = transform;

        Debug.Log("배리어가 활성화되었습니다.");

        // 배리어 지속 시간 관리
        StartCoroutine(BarrierDurationCoroutine());

        // 쿨타임 시작
        StartCoroutine(StartBarrierCooldown());
        SoundManager.Instance.PlaySound(ActivateBarrierSound);
    }

    void DeactivateBarrier()
    {
        if (!hasBarrier)
            return; // 배리어가 이미 해체된 상태라면 아무 작업도 하지 않음

        hasBarrier = false;

        if (activeBarrierEffect != null)
        {
            ParticleSystem ps = activeBarrierEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                ps.Clear();
            }
            Destroy(activeBarrierEffect);
            activeBarrierEffect = null;
        }

        SoundManager.Instance.PlaySound(DeactivateBarrierSound); // 해체 소리 재생
        Debug.Log("배리어가 해제되었습니다.");
    }

    IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(effect);
    }

    IEnumerator BarrierDurationCoroutine()
    {
        yield return new WaitForSeconds(playerStats.barrierDuration);

        if (hasBarrier) // 배리어가 활성 상태인지 확인
        {
            DeactivateBarrier();
        }
    }

    IEnumerator StartBarrierCooldown()
    {
        isBarrierOnCooldown = true;
        yield return new WaitForSeconds(playerStats.barrierCooldown); // PlayerStats에서 쿨타임 참조
        isBarrierOnCooldown = false;
        Debug.Log("배리어 스킬이 다시 사용 가능합니다.");
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
        SoundManager.Instance.PlaySound(LayDownAreaSound);
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
        SoundManager.Instance.PlaySound(MeteorSound);
    }

    private IEnumerator UseShockwave()
    {
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Z값을 0으로 설정 (2D 게임인 경우)

        // 스폰 포인트와 마우스 위치 간의 방향 계산
        Vector3 direction = (mousePosition - spawnPoint.position).normalized;

        // 쇼크웨이브 이펙트 생성
        GameObject shockwaveEffect = Instantiate(shockwaveEffectPrefab, spawnPoint.position, Quaternion.identity);

        // Y좌표는 스폰포인트에서 변하지 않도록 설정
        shockwaveEffect.transform.position = new Vector3(shockwaveEffect.transform.position.x, spawnPoint.position.y, shockwaveEffect.transform.position.z);

        // 마우스 커서 방향으로 회전 (회전값을 Quaternion을 사용하여 설정)
        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, direction); // 2D에서 z축 기준으로 회전
        shockwaveEffect.transform.rotation = rotation;

        // 쇼크웨이브 이펙트가 일정 시간 후 사라지도록 설정
        Destroy(shockwaveEffect, shockwaveDuration);

        // 코루틴 종료를 위해 반드시 null을 반환해야 함
        yield return null;
    }

    IEnumerator RollTowardsCursor()
    {
        lastRollTime = Time.time;

        animator.SetTrigger("Roll");
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
        if (hasBarrier)
        {
            DeactivateBarrier(); // 배리어 해제
            return;
        }

        animator.SetTrigger("CharacterHit");
        if (!isInvincible)
        {
            StartCoroutine(Knockback(hitDirection));
            StartCoroutine(InvincibilityCoroutine());
        }
        PlayerHealthManager.Instance.TakeDamage(damage);
        StartCoroutine(InvincibilityAndBlinking());
        SoundManager.Instance.PlaySound(AttackedSound);
    }
    private IEnumerator Knockback(Vector3 hitDirection)
    {
        Vector3 knockbackDir = hitDirection.normalized;
        knockbackDir.y = 0;
        float knockbackTime = 0.1f;

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
        isInvincible = true;  
        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
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
        SoundManager.Instance.PlaySound(DeathSound);
    }
}