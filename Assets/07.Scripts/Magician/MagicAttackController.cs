using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackController : MonoBehaviour
{
    public PlayerStats playerStats; // 플레이어 스탯 참조
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
        // 몬스터 또는 보스일 경우
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            BossController boss = other.GetComponent<BossController>();

            if (monster != null)
            {
                // 몬스터의 체력 감소
                monster.TakeDamage((int)playerStats.magicAttackDamage);
            }
            else if (boss != null)
            {
                // 보스의 체력 감소
                boss.TakeDamage((int)playerStats.magicAttackDamage);
            }

            // 히트 이펙트 생성
            if (hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                Destroy(hitEffect, 1f);
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