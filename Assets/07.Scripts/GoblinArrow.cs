using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinArrow : MonoBehaviour
{
    public float damage = 10f; // 화살 데미지

    void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            // 플레이어의 TakeDamage 메서드를 호출해서 데미지를 입힘
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // 화살이 플레이어와 충돌하면 충돌 방향을 계산
                Vector3 hitDirection = (other.transform.position - transform.position).normalized;

                // TakeDamage 메서드 호출 (충돌 방향과 데미지 전달)
                playerController.TakeDamage(hitDirection, damage);
            }

            // 화살 사라짐
            Destroy(gameObject);
        }
    }
}