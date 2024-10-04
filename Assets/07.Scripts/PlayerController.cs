using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterController : MonoBehaviour
{
    public NavMeshAgent agent;  // ĳ������ NavMeshAgent
    public Animator animator;   // ĳ������ Animator
    public Camera mainCamera;   // ���� ī�޶�

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
        // ���콺 ��Ŭ�� ����
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���� Ŭ���� ��� ĳ���� �̵�
            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }

        // ĳ���Ͱ� �̵� ������ Ȯ��
//        if (agent.remainingDistance > agent.stoppingDistance && agent.velocity.magnitude > 0.1f)
//        {
//            animator.SetBool("isWalking", true);  // �̵� ���� �� �ȴ� �ִϸ��̼� ���
//        }
//       else
//        {
//            animator.SetBool("isWalking", false); // ������ �� �ִϸ��̼� ����
//        }
    }
}
