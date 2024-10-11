using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public int damage = 20; // 파이어볼 데미지
    public float lifetime = 5f; // 파이어볼이 유지되는 시간
    public float speed = 10f; // 파이어볼 속도

    private Vector3 direction;

    void Start()
    {
        // 일정 시간이 지나면 파이어볼을 파괴
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 파이어볼이 직선으로 날아가도록
        transform.Translate(direction * speed * Time.deltaTime);
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
            }

            // 파이어볼을 파괴
            Destroy(gameObject);
        }
    }
}
