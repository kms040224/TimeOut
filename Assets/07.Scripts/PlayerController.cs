using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    public NavMeshAgent agent;  // 캐릭터의 NavMeshAgent
    public Animator animator;   // 캐릭터의 Animator
    public Camera mainCamera;   // 메인 카메라

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
                agent.SetDestination(hit.point);
            }
        }

        // 캐릭터가 이동 중인지 확인
//        if (agent.remainingDistance > agent.stoppingDistance && agent.velocity.magnitude > 0.1f)
//        {
//            animator.SetBool("isWalking", true);  // 이동 중일 때 걷는 애니메이션 재생
//        }
//       else
//        {
//            animator.SetBool("isWalking", false); // 멈췄을 때 애니메이션 중지
//        }
    }
}
