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
    public Transform leftEye;                 // 보스 왼쪽 눈의 위치
    public Transform rightEye;                // 보스 오른쪽 눈의 위치
    public GameObject laserPrefab;           // 레이저 프리팹
    public float laserDuration = 2f;         // 레이저 발사 지속 시간
    public float laserCooldown = 3f;         // 레이저 쿨타임

    private bool isLaserActive = false;      // 레이저 활성화 여부
    private Coroutine laserCoroutine;        // 레이저 코루틴

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
        while (isAlive)                          // 보스가 살아있는 동안 반복
        {
            yield return PatternSequence();      // 패턴 순차 실행
        }
    }

    private IEnumerator PatternSequence()
    {
        yield return JumpPattern();              // 점프 패턴 실행
        yield return RockDropPattern();          // 돌 떨어뜨리기 패턴 실행
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
        bossAnimator.SetTrigger("RockDrop"); // 돌 떨어뜨리기 애니메이션 실행

        yield return new WaitForSeconds(1f); // 애니메이션 시전 대기

        List<int> chosenIndices = new List<int>(); // 중복되지 않도록 선택된 인덱스를 기록할 리스트
        List<Vector3> dropPositions = new List<Vector3>(); // 드랍 위치를 저장할 리스트
        while (chosenIndices.Count < 4) // 4번 반복하여 돌이 떨어질 4개의 위치를 결정
        {
            int randomIndex = Random.Range(0, rockDropPositions.Length); // 랜덤한 드랍 포지션 인덱스
            if (!chosenIndices.Contains(randomIndex)) // 중복된 위치는 선택하지 않음
            {
                chosenIndices.Add(randomIndex); // 선택된 인덱스 기록
                dropPositions.Add(rockDropPositions[randomIndex].position); // 드랍 위치 저장
            }
        }

        // 경고 표시를 드랍 위치에 먼저 1초 전에 표시
        foreach (Vector3 dropPosition in dropPositions)
        {
            ShowDropWarning(dropPosition);
        }

        // 1초 후에 돌을 각 드랍 위치에 떨어뜨리기
        yield return new WaitForSeconds(1f); // 경고 표시 후 대기

        // 각 드랍 포지션에 돌을 동시에 떨어뜨리기
        foreach (Vector3 dropPosition in dropPositions)
        {
            Instantiate(rockPrefab, dropPosition, Quaternion.identity); // 돌 생성
        }

        yield return new WaitForSeconds(rockDropDelay); // 돌 떨어진 후 대기
    }

    private void ShowDropWarning(Vector3 position)
    {
        // 경고 표시를 위해 미리 지정된 경고 프리팹을 생성
        GameObject warningIndicator = Instantiate(rockIndicatorPrefab, position, Quaternion.identity);
        warningIndicator.transform.position = new Vector3(position.x, 0.1f, position.z);
        Destroy(warningIndicator, 1f); // 1초 후 경고 표시 제거
    }

    private IEnumerator LaserPattern()
    {
        int laserCount = 0; // 레이저 발사 횟수 추적

        while (laserCount < 2) // 레이저를 2번 발사
        {
            // 플레이어 위치를 타겟으로 설정
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                FireLaser(player.transform.position); // 레이저 발사
            }

            laserCount++;

            // 레이저 발사 후 쿨타임 대기
            yield return new WaitForSeconds(laserCooldown);
        }

        // 레이저를 2번 발사한 후, 처음 패턴으로 돌아감
        yield return new WaitForSeconds(laserCooldown); // 레이저 쿨타임 기다린 후
        StartCoroutine(PatternSequence()); // 처음 패턴으로 돌아가기
    }

    private void FireLaser(Vector3 targetPosition)
    {
        // 왼쪽 눈에서 레이저 발사
        GameObject leftLaser = Instantiate(laserPrefab, leftEye.position, Quaternion.identity);
        LineRenderer leftLine = leftLaser.GetComponent<LineRenderer>();
        leftLine.SetPosition(0, leftEye.position);

        // 오른쪽 눈에서 레이저 발사
        GameObject rightLaser = Instantiate(laserPrefab, rightEye.position, Quaternion.identity);
        LineRenderer rightLine = rightLaser.GetComponent<LineRenderer>();
        rightLine.SetPosition(0, rightEye.position);

        // 동일한 목표 위치로 레이저 발사
        StartCoroutine(ShootLaser(leftLine, leftEye.position, targetPosition));
        StartCoroutine(ShootLaser(rightLine, rightEye.position, targetPosition));
    }

    private IEnumerator ShootLaser(LineRenderer lineRenderer, Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        float laserDuration = 1f; // 레이저 발사 지속 시간

        // 레이저가 목표 위치로 향하는 동안 동일한 비율로 보간
        while (elapsedTime < laserDuration)
        {
            float t = elapsedTime / laserDuration; // 시간 비율을 계산하여 0~1로 보간
            Vector3 currentTarget = Vector3.Lerp(startPosition, targetPosition, t); // 목표 위치로 보간
            lineRenderer.SetPosition(1, currentTarget); // 레이저 끝 위치 업데이트

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 레이저가 목표 위치에 도달한 후, 수평 방향으로 계속 이동
        StartCoroutine(ShootLaserHorizontally(lineRenderer, lineRenderer.GetPosition(1)));
    }

    private IEnumerator ShootLaserHorizontally(LineRenderer lineRenderer, Vector3 startPosition)
    {
        float duration = 2f; // 레이저를 오른쪽/왼쪽으로 이동시키는 시간
        float elapsedTime = 0f;

        // 카메라 기준으로 좌우 방향 벡터를 계산
        Vector3 cameraForward = Camera.main.transform.forward;  // 카메라의 앞 방향
        Vector3 cameraRight = Camera.main.transform.right;      // 카메라의 오른쪽 방향 (좌우 방향)

        // 카메라의 앞 방향을 제외한 좌우 방향으로만 이동하도록 설정
        Vector3 direction = Vector3.Cross(cameraForward, Vector3.up).normalized;  // 수평 방향만 취함

        // 두 레이저가 동일한 방향으로 이동하도록 처리
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // 시간 비율을 계산하여 0~1로 보간
            Vector3 currentTarget = startPosition + direction * t * 10f; // 이동 거리 설정
            lineRenderer.SetPosition(1, currentTarget); // 레이저 끝 위치 업데이트

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 레이저가 끝난 후 비활성화 또는 삭제
        Destroy(lineRenderer.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector3 hitDirection = other.transform.position - transform.position; // 충돌 방향 계산
                player.TakeDamage(hitDirection, collisionDamage); // 플레이어에게 데미지 및 넉백 적용
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