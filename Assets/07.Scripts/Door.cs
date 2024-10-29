using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public Vector3 cameraPosition; // ī�޶� �̵��� ��ġ
    public Vector3 cameraRotation; // ī�޶� �ٶ� ���� (EulerAngles)
    public float fadeDuration = 1.0f; // ���̵� ��/�ƿ� �ð�
    public FadeController fadeController; // ���̵� ��Ʈ�ѷ� ����
    public Camera mainCamera; // ���� ī�޶� ����
    public Transform destination;
    public NavMeshSurface navMeshSurface;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ������ ����
        {
            // �÷��̾��� �ִϸ����͸� Idle ���·� ��ȯ
            Animator playerAnimator = other.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("isMoving", false); // �̵� ���� �ƴ��� �����Ͽ� Idle �ִϸ��̼� ���
                playerAnimator.Play("Idle"); // Idle �ִϸ��̼����� ��ȯ
            }

            StartCoroutine(TeleportPlayer(other.transform));
            UpdateNavMesh();
            StartCoroutine(DisablePlayerMovement(other.GetComponent<PlayerController>()));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        // �÷��̾� Collider ��Ȱ��ȭ
        Collider playerCollider = player.GetComponent<Collider>();
        playerCollider.enabled = false;

        // ���̵� ��
        yield return StartCoroutine(fadeController.FadeIn(fadeDuration));

        // �÷��̾� �̵�
        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            // ���ο� ��ġ�� �����̵�
            agent.Warp(destination.position);

            // ������Ʈ�� ���¸� �ʱ�ȭ
            agent.ResetPath(); // ��θ� �ʱ�ȭ�Ͽ� ���¸� ����
            agent.isStopped = true; // ������ ��Ȱ��ȭ
        }

        // ī�޶� �̵�
        mainCamera.transform.position = cameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation); // ȸ�� ���� ����

        // ���̵� �ƿ�
        yield return StartCoroutine(fadeController.FadeOut(fadeDuration));

        // �÷��̾� Collider Ȱ��ȭ
        playerCollider.enabled = true;

        // ���� �̵� ���������� �̵��� ����
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.shouldMove = false; // shouldMove�� false�� �����Ͽ� �̵� ����
        }
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // NavMesh�� ����
        }
    }
    private IEnumerator DisablePlayerMovement(PlayerController player)
    {
        // �÷��̾��� �������� ��Ȱ��ȭ
        player.enabled = false;

        // 2�� ���
        yield return new WaitForSeconds(2f);

        // �÷��̾��� �������� �ٽ� Ȱ��ȭ
        player.enabled = true;
    }
}