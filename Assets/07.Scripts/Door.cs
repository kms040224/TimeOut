using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform targetPosition; // �÷��̾ �̵��� ��ġ
    public Vector3 cameraPosition; // ī�޶� �̵��� ��ġ
    public Vector3 cameraRotation; // ī�޶� �ٶ� ���� (EulerAngles)
    public float fadeDuration = 1.0f; // ���̵� ��/�ƿ� �ð�
    public FadeController fadeController; // ���̵� ��Ʈ�ѷ� ����
    public Camera mainCamera; // ���� ī�޶� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // �÷��̾ ������ ����
        {
            StartCoroutine(TeleportPlayer(other.transform));
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
        player.position = targetPosition.position;

        // ī�޶� �̵�
        mainCamera.transform.position = cameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation); // ȸ�� ���� ����

        // ���̵� �ƿ�
        yield return StartCoroutine(fadeController.FadeOut(fadeDuration));

        // �÷��̾� Collider Ȱ��ȭ
        playerCollider.enabled = true;
    }
}