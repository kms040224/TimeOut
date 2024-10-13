using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // 카메라가 따라다닐 캐릭터
    public Vector3 offset;        // 카메라와 캐릭터 사이의 거리
    public float smoothSpeed = 0.125f;  // 카메라의 움직임을 부드럽게 하기 위한 속도
    public float rotationAngle = 45f;   // 카메라의 쿼터뷰 회전 각도
    public float height = 5f;     // 카메라가 캐릭터 위로 얼마나 높이 배치될지

    public float zoomSpeed = 2f;  // 줌인/아웃 속도
    public float minZoom = 2f;    // 줌인 제한
    public float maxZoom = 10f;   // 줌아웃 제한

    private float currentZoom = 5f; // 현재 줌 값

    void LateUpdate()
    {
        if (target == null) return;

        // 마우스 휠로 줌인/아웃 값 변경
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scrollInput * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom); // 줌 범위 제한

        // 캐릭터의 위치에 따른 카메라의 새로운 위치 계산
        Vector3 desiredPosition = target.position + Quaternion.Euler(0, rotationAngle, 0) * offset * currentZoom;
        desiredPosition.y += height;  // 카메라 높이 설정

        // 부드럽게 카메라를 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 항상 캐릭터를 바라보도록 설정
        transform.LookAt(target);
    }
}
