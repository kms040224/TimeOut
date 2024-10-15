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

    private void LateUpdate()
    {
        if (target == null) return;

        // 캐릭터의 위치에 따른 카메라의 새로운 위치 계산
        Vector3 desiredPosition = target.position + Quaternion.Euler(0, rotationAngle, 0) * offset;
        desiredPosition.y += height;  // 카메라 높이 설정

        // 부드럽게 카메라를 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 카메라가 항상 캐릭터를 바라보도록 설정
        transform.LookAt(target);
    }
}
