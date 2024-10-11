using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrbController : MonoBehaviour
{
    public float speed = 5f;  // 오브의 이동 속도
    public int damage = 20;   // 오브가 몬스터에게 입히는 데미지
    private Transform target; // 가장 가까운 몬스터 타겟

    // 가장 가까운 몬스터 설정
    public void SetTarget(Transform monsterTarget)
    {
        target = monsterTarget;
    }

    void Update()
    {
        // 타겟이 없으면 오브 파괴
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 타겟을 향해 이동
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // 몬스터와 충돌 시 데미지 입힘
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();

            if (monster != null)
            {
                monster.TakeDamage(damage); // 몬스터에게 데미지 입힘
                Debug.Log("FireOrb hit the monster, dealing " + damage + " damage.");
                Destroy(gameObject); // 충돌 후 파이어 오브 삭제
            }
        }
    }
}
