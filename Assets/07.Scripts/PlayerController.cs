using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;   // 캐릭터의 Animator
    public Camera mainCamera;   // 메인 카메라
    public GameObject fireballPrefab;  // 발사할 파이어볼 프리팹
    public GameObject flamethrowerPrefab; // 사용할 화염방사기 프리팹
    public GameObject GameOverPanel;
    public Transform fireballSpawnPoint;  // 파이어볼이 발사될 위치
    public Transform flamethrowerSpawnPoint; // 화염방사기 발사 위치
    public float movementSpeed = 10f;    // 이동 속도
    public float rotationSpeed = 10f;    // 회전 속도
    public float flamethrowerDuration = 1.5f; // 화염방사기 지속 시간
    public float flamethrowerCooldown = 12f; // 화염방사기 쿨타임
    public int health = 100;

    private Vector3 destinationPoint;    // 이동할 목표 지점
    private bool shouldMove = false;     // 이동 중 여부
    private bool isUsingFlamethrower = false; // 화염방사기 사용 중 여부
    private float flamethrowerCooldownTimer = 0f; // 화염방사기 쿨타임 타이머


    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (mainCamera == null)
            mainCamera = Camera.main;
        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (health <= 0)
        {
            return;
        }

        // 쿨타임이 끝났고 화염방사기를 사용 중이 아닐 때 Q 키 감지
        if (Input.GetKeyDown(KeyCode.Q) && !isUsingFlamethrower && flamethrowerCooldownTimer <= 0)
        {
            AimAtCursor(); // 마우스 커서 방향으로 즉시 캐릭터 회전
            StartCoroutine(UseFlamethrower());
        }

        // 쿨타임 타이머 업데이트
        if (flamethrowerCooldownTimer > 0)
        {
            flamethrowerCooldownTimer -= Time.deltaTime;
        }

        // 마우스 우클릭 감지 (이동)
        if (Input.GetMouseButtonDown(1) && !isUsingFlamethrower)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 땅을 클릭한 경우 캐릭터 이동
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    destinationPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                    shouldMove = true;
                }
            }
        }

        // 이동 관련 처리
        if (shouldMove && !isUsingFlamethrower)
        {
            // 목표 지점을 향해 회전
            Quaternion targetRotation = Quaternion.LookRotation(destinationPoint - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 목표 지점으로 이동
            transform.position = Vector3.MoveTowards(transform.position, destinationPoint, movementSpeed * Time.deltaTime);

            // 캐릭터가 도착했을 때 이동 멈춤
            if (transform.position == destinationPoint)
            {
                shouldMove = false;
            }

            // 걷는 애니메이션 활성화
            animator.SetBool("isWalking", true);
        }
        else
        {
            // 캐릭터가 멈췄을 때 애니메이션 중지
            animator.SetBool("isWalking", false);
        }

        // A 키를 눌렀을 때 파이어볼 발사 및 캐릭터 회전
        if (Input.GetKeyDown(KeyCode.A) && !isUsingFlamethrower)
        {
            AimAtCursor(); // 마우스 커서 방향으로 즉시 캐릭터 회전
            ShootFireball(); // 파이어볼 발사
        }
    }

    // 마우스 커서 방향으로 캐릭터 회전
    void AimAtCursor()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 마우스 클릭 지점과 캐릭터의 위치 차이로 회전 방향 설정
            Vector3 direction = (hit.point - transform.position).normalized;
            direction.y = 0;  // Y축 회전 제거

            // 회전할 방향을 LookRotation으로 설정
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            // 캐릭터의 회전을 즉시 적용
            transform.rotation = lookRotation;
        }
    }

    // 화염방사기 사용 코루틴
    private IEnumerator UseFlamethrower()
    {
        isUsingFlamethrower = true;

        // 화염방사기 애니메이션 시작
        animator.SetBool("isUsingFlamethrower", true);

        // 마우스 커서 방향으로 화염방사기 발사
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 화염방사기 프리팹을 생성하여 발사
            GameObject flamethrower = Instantiate(flamethrowerPrefab, flamethrowerSpawnPoint.position, Quaternion.identity);
            flamethrower.transform.LookAt(hit.point); // 화염방사기를 마우스 커서 방향으로 회전

            // 화염방사기 사용 시간 대기
            yield return new WaitForSeconds(flamethrowerDuration);
        }

        // 화염방사기 애니메이션 종료
        animator.SetBool("isUsingFlamethrower", false);

        // 쿨타임 설정
        flamethrowerCooldownTimer = flamethrowerCooldown;

        // 화염방사기 사용 종료
        isUsingFlamethrower = false;
    }

    // 파이어볼 발사
    void ShootFireball()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 마우스 커서가 닿은 지점을 계산
        if (Physics.Raycast(ray, out hit))
        {
            // 파이어볼을 생성하고 발사 방향 설정
            GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, Quaternion.identity);

            // 파이어볼의 FireballController 스크립트를 가져와서 발사
            FireballController fireballController = fireball.GetComponent<FireballController>();
            if (fireballController != null)
            {
                fireballController.Launch(hit.point); // 마우스 커서 위치 전달
            }

            Debug.Log("Fireball shot towards: " + hit.point);
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage; // 체력 감소
        Debug.Log("Player took damage! Current health: " + health);

        // 체력이 0 이하가 되면 플레이어 사망 처리
        if (health <= 0)
        {
            Die();
        }
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
