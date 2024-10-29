using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public Vector3 cameraPosition; // 카메라가 이동할 위치
    public Vector3 cameraRotation; // 카메라가 바라볼 각도 (EulerAngles)
    public float fadeDuration = 1.0f; // 페이드 인/아웃 시간
    public FadeController fadeController; // 페이드 컨트롤러 연결
    public Camera mainCamera; // 메인 카메라 연결
    public Transform destination;
    public NavMeshSurface navMeshSurface;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 닿으면 실행
        {
            // 플레이어의 애니메이터를 Idle 상태로 전환
            Animator playerAnimator = other.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isMoving", false); // 이동 중이 아님을 설정하여 Idle 애니메이션 재생
                playerAnimator.Play("Idle"); // Idle 애니메이션으로 전환
            }

            StartCoroutine(TeleportPlayer(other.transform));
            UpdateNavMesh();
            StartCoroutine(DisablePlayerMovement(other.GetComponent<PlayerController>()));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        // 플레이어 Collider 비활성화
        Collider playerCollider = player.GetComponent<Collider>();
        playerCollider.enabled = false;

        // 페이드 인
        yield return StartCoroutine(fadeController.FadeIn(fadeDuration));

        // 플레이어 이동
        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // 새로운 위치로 순간이동
            agent.Warp(destination.position);

            // 에이전트의 상태를 초기화
            agent.ResetPath(); // 경로를 초기화하여 상태를 리셋
            agent.isStopped = true; // 움직임 비활성화
        }

        // 카메라 이동
        mainCamera.transform.position = cameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation); // 회전 각도 설정

        // 페이드 아웃
        yield return StartCoroutine(fadeController.FadeOut(fadeDuration));

        // 플레이어 Collider 활성화
        playerCollider.enabled = true;

        // 이전 이동 지점으로의 이동을 중지
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.shouldMove = false; // shouldMove를 false로 설정하여 이동 중지
        }
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // NavMesh를 재계산
        }
    }
    private IEnumerator DisablePlayerMovement(PlayerController player)
    {
        // 플레이어의 움직임을 비활성화
        player.enabled = false;

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // 플레이어의 움직임을 다시 활성화
        player.enabled = true;
    }
}