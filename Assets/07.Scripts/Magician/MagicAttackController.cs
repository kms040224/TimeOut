using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackController : MonoBehaviour
{
    public int damage = 20; // 파이어볼 데미지
    public float lifetime = 5f; // 파이어볼이 유지되는 시간
    public float speed = 10f; // 파이어볼 속도
    public GameObject hitEffectPrefab; // 타격 시 생성할 이펙트 프리팹

    private Vector3 direction;
    private float lifetimeTimer;

    void OnEnable()
    {
        // 오브젝트가 활성화될 때마다 lifetime 타이머 초기화
        lifetimeTimer = lifetime;
    }

    void Update()
    {
        // 파이어볼이 직선으로 날아가도록
        transform.Translate(direction * speed * Time.deltaTime);

        // 수명이 다하면 오브젝트 풀로 반환
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            ReturnToPool();
        }
    }

    public void Launch(Vector3 target)
    {
        // 타겟 방향 설정
        direction = (target - transform.position).normalized;
        direction.y = 0; // Y축 제거로 수평 이동
    }

    // 충돌 감지
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 몬스터일 경우
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                // 몬스터의 체력을 줄임
                monster.TakeDamage(damage);

                // 히트 이펙트 생성
                if (hitEffectPrefab != null)
                {
                    GameObject hitEffect = Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                    Destroy(hitEffect, 1f); // 히트 이펙트의 복제본은 일정 시간 후 파괴
                }
            }

            // 파이어볼을 풀로 반환
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        // 오브젝트 풀로 반환
        ObjectPool.Instance.ReturnMagicAttack(gameObject);
    }
}
