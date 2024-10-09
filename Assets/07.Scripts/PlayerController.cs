using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator animator;   // 캐릭터의 Animator
    public Camera mainCamera;   // 메인 카메라
    public GameObject fireballPrefab;  // 발사할 파이어볼 프리팹
    public Transform fireballSpawnPoint;  // 파이어볼이 발사될 위치
    public float movementSpeed = 10f;    // 이동 속도
    public float rotationSpeed = 10f;    // 회전 속도
    private Vector3 destinationPoint;    // 이동할 목표 지점
    private bool shouldMove = false;     // 이동 중 여부

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // 마우스 우클릭 감지 (이동)
        if (Input.GetMouseButtonDown(1))
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
        if (shouldMove)
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

        // Q 키를 눌렀을 때 파이어볼 발사 및 캐릭터 회전
        if (Input.GetKeyDown(KeyCode.A))
        {
            AimAtCursorInstantly(); // 마우스 커서 방향으로 즉시 캐릭터 회전
            ShootFireball(); // 파이어볼 발사
        }
    }

    // 마우스 커서 방향으로 캐릭터 즉시 회전
    void AimAtCursorInstantly()
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
}
