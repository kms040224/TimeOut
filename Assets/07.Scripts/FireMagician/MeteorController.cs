using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    public PlayerStats playerStats;
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

    void Update()
    {
        transform.position += targetDirection * speed * Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDirection, out hit, speed * Time.deltaTime))
        {
            MonsterController monster = hit.collider.GetComponent<MonsterController>();
            if (monster != null && !damagedMonsters.Contains(monster))
            {
                int calculatedDamage = (int)(playerStats.magicAttackDamage * playerStats.meteorDamageMultiplier);
                monster.TakeDamage(calculatedDamage); // 배율 적용
                damagedMonsters.Add(monster);
            }
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject); // 생명주기 종료 후 메테오 오브젝트 삭제
    }
}
