using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearObstacle : MonoBehaviour
{
    [SerializeField] private float riseHeight = 2.0f; // 창이 올라갈 높이
    [SerializeField] private float riseSpeed = 5.0f;  // 창이 올라가는 속도
    [SerializeField] private float fallSpeed = 2.0f;  // 창이 내려가는 속도
    [SerializeField] private float interval = 5.0f;   // 창이 올라오는 간격
    [SerializeField] private int damage = 10;         // 플레이어에게 줄 데미지

    private Vector3 initialPosition;
    private Vector3 targetPosition;

    void Start()
    {
        initialPosition = transform.position;
        targetPosition = initialPosition + Vector3.up * riseHeight;
        StartCoroutine(ActivateSpear());
    }

    private IEnumerator ActivateSpear()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            // 창 올리기 (빠르게)
            yield return MoveToPosition(targetPosition, riseSpeed);

            // 잠시 대기 (위에서 멈춘 상태)
            yield return new WaitForSeconds(0.5f);

            // 창 내리기 (느리게)
            yield return MoveToPosition(initialPosition, fallSpeed);
        }
    }

    private IEnumerator MoveToPosition(Vector3 target, float speed)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            Vector3 hitDirection = (player.transform.position - transform.position).normalized; // 창에서 플레이어 방향 계산
            float damage = 5f; // 줄 데미지 설정
            player.TakeDamage(hitDirection, damage); // TakeDamage 메서드 호출
        }
    }
}