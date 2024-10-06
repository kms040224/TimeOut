using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    public NavMeshAgent agent;  // 캐릭터의 NavMeshAgent
    public Animator animator;   // 캐릭터의 Animator
    public Camera mainCamera;   // 메인 카메라
    public GameObject fireballPrefab;  // 발사할 파이어볼 프리팹
    public Transform fireballSpawnPoint;  // 파이어볼이 발사될 위치
    public float fireballSpeed = 10f;    // 파이어볼 속도

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        // 마우스 우클릭 감지
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 땅을 클릭한 경우 캐릭터 이동
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    agent.SetDestination(hit.point);
                }
            }
        }

        // Q 키를 눌렀을 때 파이어볼 발사 및 캐릭터 회전
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AimAtCursor(); // 마우스 커서 방향으로 캐릭터 회전
            ShootFireball(); // 파이어볼 발사
        }

        // 캐릭터가 이동 중인지 확인하여 애니메이션 설정
        if (agent.remainingDistance > agent.stoppingDistance && agent.velocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);  // 이동 중일 때 걷는 애니메이션 재생
        }
        else
        {
            animator.SetBool("isWalking", false); // 멈췄을 때 애니메이션 중지
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
            // 캐릭터의 회전을 즉시 업데이트
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

            // 마우스 커서가 가리키는 방향으로 파이어볼을 날아가게 설정
            Vector3 fireballDirection = (hit.point - fireballSpawnPoint.position).normalized;
            fireballDirection.y = 0; // 쿼터뷰에서 Y축(높이)을 고정하여 수평으로만 이동하게 설정

            fireball.GetComponent<Rigidbody>().velocity = fireballDirection * fireballSpeed;

            Debug.Log("Fireball shot towards: " + hit.point);
        }
    }
}