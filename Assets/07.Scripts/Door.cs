using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform targetPosition; // 플레이어가 이동할 위치
    public Vector3 cameraPosition; // 카메라가 이동할 위치
    public Vector3 cameraRotation; // 카메라가 바라볼 각도 (EulerAngles)
    public float fadeDuration = 1.0f; // 페이드 인/아웃 시간
    public FadeController fadeController; // 페이드 컨트롤러 연결
    public Camera mainCamera; // 메인 카메라 연결

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 닿으면 실행
        {
            StartCoroutine(TeleportPlayer(other.transform));
        }
    }

    private IEnumerator TeleportPlayer(Transform player)
    {
        // 페이드 인
        yield return StartCoroutine(fadeController.FadeIn(fadeDuration));

        // 플레이어 이동
        player.position = targetPosition.position;

        // 카메라 이동
        mainCamera.transform.position = cameraPosition;
        mainCamera.transform.rotation = Quaternion.Euler(cameraRotation); // 회전 각도 설정

        // 페이드 아웃
        yield return StartCoroutine(fadeController.FadeOut(fadeDuration));
    }
}