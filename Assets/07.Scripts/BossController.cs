using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float jumpHeight = 5f;             // ���� ����
    public float jumpCooldown = 1f;           // ���� ���� (1��)
    public int jumpCount = 4;                 // �� ���� Ƚ��
    public Transform mapCenter;               // �� �߾� ��ġ (�ʱ� ���� ��ġ)
    public GameObject jumpIndicatorPrefab;    // ���� ��ġ ǥ�� ������

    private int currentJump = 0;              // ���� ���� Ƚ��
    private Vector3 initialPosition;          // ������ �ʱ� ��ġ

    private void Start()
    {
        initialPosition = transform.position; // ������ �ʱ� ��ġ ����
        StartCoroutine(JumpPattern());        // ���� ���� ����
    }

    private IEnumerator JumpPattern()
    {
        while (currentJump < jumpCount)
        {
            Vector3 targetPosition;

            if (currentJump < jumpCount - 1)
            {
                // ó�� 3���� �÷��̾ ���� ����
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    targetPosition = player.transform.position;
                }
                else
                {
                    yield break; // �÷��̾ ���� ��� ���� ����
                }
            }
            else
            {
                // ������ ������ �� �߾����� ����
                targetPosition = mapCenter.position;
            }

            // ���� ��ġ�� ������ ǥ�� ����
            GameObject indicator = Instantiate(jumpIndicatorPrefab, targetPosition, Quaternion.identity);
            Destroy(indicator, jumpCooldown); // ǥ�� ���� �ð� ����

            // ���� ����
            yield return JumpToPosition(targetPosition);

            currentJump++;
            yield return new WaitForSeconds(jumpCooldown); // ���� ���� ���
        }

        currentJump = 0; // ���� Ƚ�� �ʱ�ȭ
    }

    private IEnumerator JumpToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 jumpPeak = (startPosition + targetPosition) / 2 + Vector3.up * jumpHeight;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(Vector3.Lerp(startPosition, jumpPeak, elapsedTime), Vector3.Lerp(jumpPeak, targetPosition, elapsedTime), elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // ��ǥ ��ġ�� ���� ��ġ ����
    }
}