using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    public Transform targetPosition; // �÷��̾ �̵��� ��ġ
    public Vector3 cameraPosition; // ī�޶� �̵��� ��ġ
    public Vector3 cameraRotation; // ī�޶� �ٶ� ���� (EulerAngles)
    public float fadeDuration = 1.0f; // ���̵� ��/�ƿ� �ð�
    public FadeController fadeController; // ���̵� ��Ʈ�ѷ� ����
    public Camera mainCamera; // ���� ī�޶� ����
    public Transform destination;
    public NavMeshSurface navMeshSurface;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ������ ����
        {
            StartCoroutine(TeleportPlayer(other.transform));
            UpdateNavMesh();
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

            agent.isStopped = false; // ������ Ȱ��ȭ
        }
        // ī�޶� �̵�
        mainCamera.transform.position = cameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation); // ȸ�� ���� ����

        // ���̵� �ƿ�
        yield return StartCoroutine(fadeController.FadeOut(fadeDuration));

        // �÷��̾� Collider Ȱ��ȭ
        playerCollider.enabled = true;
    }

    private void UpdateNavMesh()
    {
        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh(); // NavMesh�� ����
        }
    }
}