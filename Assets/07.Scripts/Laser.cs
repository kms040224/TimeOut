using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public int damage = 10;
    public LineRenderer lineRenderer;
    private BoxCollider boxCollider;

    private void Awake()
    {
        // BoxCollider를 동적으로 추가
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true; // 트리거로 설정
    }

    private void Update()
    {
        SyncColliderWithLineRenderer();
    }

    private void SyncColliderWithLineRenderer()
    {
        if (lineRenderer.positionCount >= 2)
        {
            // LineRenderer의 시작점과 끝점을 가져오기
            Vector3 startPoint = lineRenderer.GetPosition(0);
            Vector3 endPoint = lineRenderer.GetPosition(1);

            // BoxCollider의 중간 지점 계산
            Vector3 midPoint = (startPoint + endPoint) / 2; // 중간 지점
            float length = Vector3.Distance(startPoint, endPoint); // 레이저 길이

            // BoxCollider의 위치와 회전 설정 (중심을 (0, 0, 0)으로 고정)
            boxCollider.transform.position = midPoint;
            boxCollider.transform.rotation = Quaternion.LookRotation(endPoint - startPoint);

            // BoxCollider 크기 조정
            boxCollider.size = new Vector3(0.1f, 0.1f, length); // 0.1f는 레이저 두께
            boxCollider.center = Vector3.zero;  // BoxCollider의 중심을 (0, 0, 0)으로 설정
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Vector3 hitDirection = other.transform.position - transform.position;
                player.TakeDamage(hitDirection, damage); // 플레이어에게 데미지와 넉백 적용
            }
        }
    }
}