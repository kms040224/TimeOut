using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    public float speed = 15f; // 메테오 이동 속도
    public float damage = 50f; // 메테오 데미지
    public float lifetime = 5f; // 메테오 생명주기
    private Vector3 targetDirection;

    private HashSet<MonsterController> damagedMonsters = new HashSet<MonsterController>(); // 데미지를 입힌 몬스터 리스트

    public void Launch(Vector3 target)
    {
        // 목표 방향을 설정
        targetDirection = (target - transform.position).normalized;
        targetDirection.y = 0;
        StartCoroutine(DestroyAfterLifetime());
    }

    private void Update()
    {
        // 메테오를 목표 방향으로 계속 이동
        transform.position += targetDirection * speed * Time.deltaTime;

        // 메테오가 떨어지는 거리와 충돌 검사
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDirection, out hit, speed * Time.deltaTime))
        {
            MonsterController monster = hit.collider.GetComponent<MonsterController>();
            if (monster != null && !damagedMonsters.Contains(monster))
            {
                monster.TakeDamage((int)damage); // 몬스터에게 데미지 적용
                damagedMonsters.Add(monster); // 이미 데미지를 입힌 몬스터로 추가
            }
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject); // 생명주기 종료 후 메테오 오브젝트 삭제
    }
}
