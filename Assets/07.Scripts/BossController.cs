using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public float jumpHeight = 5f;             // 점프 높이
    public float jumpCooldown = 1f;           // 점프 간격 (1초)
    public int jumpCount = 4;                 // 총 점프 횟수
    public Transform mapCenter;               // 맵 중앙 위치 (초기 스폰 위치)
    public GameObject jumpIndicatorPrefab;    // 점프 위치 표시 프리팹

    private int currentJump = 0;              // 현재 점프 횟수
    private Vector3 initialPosition;          // 보스의 초기 위치

    private void Start()
    {
        initialPosition = transform.position; // 보스의 초기 위치 저장
        StartCoroutine(JumpPattern());        // 점프 패턴 시작
    }

    private IEnumerator JumpPattern()
    {
        while (currentJump < jumpCount)
        {
            Vector3 targetPosition;

            if (currentJump < jumpCount - 1)
            {
                // 처음 3번은 플레이어를 향해 점프
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    targetPosition = player.transform.position;
                }
                else
                {
                    yield break; // 플레이어가 없는 경우 패턴 종료
                }
            }
            else
            {
                // 마지막 점프는 맵 중앙으로 설정
                targetPosition = mapCenter.position;
            }

            // 점프 위치에 빨간색 표시 생성
            GameObject indicator = Instantiate(jumpIndicatorPrefab, targetPosition, Quaternion.identity);
            Destroy(indicator, jumpCooldown); // 표시 유지 시간 설정

            // 점프 수행
            yield return JumpToPosition(targetPosition);

            currentJump++;
            yield return new WaitForSeconds(jumpCooldown); // 점프 간격 대기
        }

        currentJump = 0; // 점프 횟수 초기화
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

        transform.position = targetPosition; // 목표 위치로 보스 위치 조정
    }
}