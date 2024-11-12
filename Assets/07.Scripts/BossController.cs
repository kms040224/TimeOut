using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float jumpHeight = 5f;
    public float jumpCooldown = 1f;
    public int jumpCount = 4;
    public int collisionDamage = 10;
    public float knockbackForce = 5f;
    public Transform mapCenter;
    public GameObject jumpIndicatorPrefab;
    public GameObject rockIndicatorPrefab;
    public GameObject rockPrefab;
    public Transform[] rockDropPositions;
    public float rockDropDelay = 1f;
    public Transform leftEye;                 // ���� ���� ���� ��ġ
    public Transform rightEye;                // ���� ������ ���� ��ġ
    public GameObject laserPrefab;           // ������ ������
    public float laserDuration = 2f;         // ������ �߻� ���� �ð�
    public float laserCooldown = 3f;         // ������ ��Ÿ��

    private bool isLaserActive = false;      // ������ Ȱ��ȭ ����
    private Coroutine laserCoroutine;        // ������ �ڷ�ƾ

    public Animator bossAnimator;
    public int health = 100;
    public float deathDelay = 2f;

    private int currentJump = 0;
    private Vector3 initialPosition;
    private bool isAlive = true;

    private void Start()
    {
        initialPosition = transform.position;
        StartCoroutine(PatternLoop());
    }

    private IEnumerator PatternLoop()
    {
        while (isAlive)                          // ������ ����ִ� ���� �ݺ�
        {
            yield return PatternSequence();      // ���� ���� ����
        }
    }

    private IEnumerator PatternSequence()
    {
        yield return JumpPattern();              // ���� ���� ����
        yield return RockDropPattern();          // �� ����߸��� ���� ����
        yield return LaserPattern();
    }

    private IEnumerator JumpPattern()
    {
        while (currentJump < jumpCount)
        {
            Vector3 targetPosition;

            if (currentJump < jumpCount - 1)
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    targetPosition = player.transform.position;
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                targetPosition = mapCenter.position;
            }

            GameObject indicator = Instantiate(jumpIndicatorPrefab, targetPosition, Quaternion.identity);
            Destroy(indicator, jumpCooldown);

            yield return JumpToPosition(targetPosition);

            currentJump++;
            yield return new WaitForSeconds(jumpCooldown);
        }

        currentJump = 0;
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

        transform.position = targetPosition;
    }

    private IEnumerator RockDropPattern()
    {
        bossAnimator.SetTrigger("RockDrop"); // �� ����߸��� �ִϸ��̼� ����

        yield return new WaitForSeconds(1f); // �ִϸ��̼� ���� ���

        List<int> chosenIndices = new List<int>(); // �ߺ����� �ʵ��� ���õ� �ε����� ����� ����Ʈ
        List<Vector3> dropPositions = new List<Vector3>(); // ��� ��ġ�� ������ ����Ʈ
        while (chosenIndices.Count < 4) // 4�� �ݺ��Ͽ� ���� ������ 4���� ��ġ�� ����
        {
            int randomIndex = Random.Range(0, rockDropPositions.Length); // ������ ��� ������ �ε���
            if (!chosenIndices.Contains(randomIndex)) // �ߺ��� ��ġ�� �������� ����
            {
                chosenIndices.Add(randomIndex); // ���õ� �ε��� ���
                dropPositions.Add(rockDropPositions[randomIndex].position); // ��� ��ġ ����
            }
        }

        // ��� ǥ�ø� ��� ��ġ�� ���� 1�� ���� ǥ��
        foreach (Vector3 dropPosition in dropPositions)
        {
            ShowDropWarning(dropPosition);
        }

        // 1�� �Ŀ� ���� �� ��� ��ġ�� ����߸���
        yield return new WaitForSeconds(1f); // ��� ǥ�� �� ���

        // �� ��� �����ǿ� ���� ���ÿ� ����߸���
        foreach (Vector3 dropPosition in dropPositions)
        {
            Instantiate(rockPrefab, dropPosition, Quaternion.identity); // �� ����
        }

        yield return new WaitForSeconds(rockDropDelay); // �� ������ �� ���
    }

    private void ShowDropWarning(Vector3 position)
    {
        // ��� ǥ�ø� ���� �̸� ������ ��� �������� ����
        GameObject warningIndicator = Instantiate(rockIndicatorPrefab, position, Quaternion.identity);
        warningIndicator.transform.position = new Vector3(position.x, 0.1f, position.z);
        Destroy(warningIndicator, 1f); // 1�� �� ��� ǥ�� ����
    }

    private IEnumerator LaserPattern()
    {
        int laserCount = 0; // ������ �߻� Ƚ�� ����

        while (laserCount < 2) // �������� 2�� �߻�
        {
            // �÷��̾� ��ġ�� Ÿ������ ����
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                FireLaser(player.transform.position); // ������ �߻�
            }

            laserCount++;

            // ������ �߻� �� ��Ÿ�� ���
            yield return new WaitForSeconds(laserCooldown);
        }

        // �������� 2�� �߻��� ��, ó�� �������� ���ư�
        yield return new WaitForSeconds(laserCooldown); // ������ ��Ÿ�� ��ٸ� ��
        StartCoroutine(PatternSequence()); // ó�� �������� ���ư���
    }

    private void FireLaser(Vector3 targetPosition)
    {
        // ���� ������ ������ �߻�
        GameObject leftLaser = Instantiate(laserPrefab, leftEye.position, Quaternion.identity);
        LineRenderer leftLine = leftLaser.GetComponent<LineRenderer>();
        leftLine.SetPosition(0, leftEye.position);

        // ������ ������ ������ �߻�
        GameObject rightLaser = Instantiate(laserPrefab, rightEye.position, Quaternion.identity);
        LineRenderer rightLine = rightLaser.GetComponent<LineRenderer>();
        rightLine.SetPosition(0, rightEye.position);

        // ������ ��ǥ ��ġ�� ������ �߻�
        StartCoroutine(ShootLaser(leftLine, leftEye.position, targetPosition));
        StartCoroutine(ShootLaser(rightLine, rightEye.position, targetPosition));
    }

    private IEnumerator ShootLaser(LineRenderer lineRenderer, Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float laserDuration = 1f; // ������ �߻� ���� �ð�

        // �������� ��ǥ ��ġ�� ���ϴ� ���� ������ ������ ����
        while (elapsedTime < laserDuration)
        {
            float t = elapsedTime / laserDuration; // �ð� ������ ����Ͽ� 0~1�� ����
            Vector3 currentTarget = Vector3.Lerp(startPosition, targetPosition, t); // ��ǥ ��ġ�� ����
            lineRenderer.SetPosition(1, currentTarget); // ������ �� ��ġ ������Ʈ

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �������� ��ǥ ��ġ�� ������ ��, ���� �������� ��� �̵�
        StartCoroutine(ShootLaserHorizontally(lineRenderer, lineRenderer.GetPosition(1)));
    }

    private IEnumerator ShootLaserHorizontally(LineRenderer lineRenderer, Vector3 startPosition)
    {
        float duration = 2f; // �������� ������/�������� �̵���Ű�� �ð�
        float elapsedTime = 0f;

        // ī�޶� �������� �¿� ���� ���͸� ���
        Vector3 cameraForward = Camera.main.transform.forward;  // ī�޶��� �� ����
        Vector3 cameraRight = Camera.main.transform.right;      // ī�޶��� ������ ���� (�¿� ����)

        // ī�޶��� �� ������ ������ �¿� �������θ� �̵��ϵ��� ����
        Vector3 direction = Vector3.Cross(cameraForward, Vector3.up).normalized;  // ���� ���⸸ ����

        // �� �������� ������ �������� �̵��ϵ��� ó��
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // �ð� ������ ����Ͽ� 0~1�� ����
            Vector3 currentTarget = startPosition + direction * t * 10f; // �̵� �Ÿ� ����
            lineRenderer.SetPosition(1, currentTarget); // ������ �� ��ġ ������Ʈ

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �������� ���� �� ��Ȱ��ȭ �Ǵ� ����
        Destroy(lineRenderer.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector3 hitDirection = other.transform.position - transform.position; // �浹 ���� ���
                player.TakeDamage(hitDirection, collisionDamage); // �÷��̾�� ������ �� �˹� ����
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0 && isAlive)
        {
            isAlive = false;
            Die();
        }
    }

    private void Die()
    {
        bossAnimator.SetTrigger("Die");
        StopAllCoroutines();

        StartCoroutine(RemoveBossAfterDelay());
    }

    private IEnumerator RemoveBossAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        gameObject.SetActive(false);
    }
}