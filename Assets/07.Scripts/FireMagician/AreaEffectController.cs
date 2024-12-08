using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectController : MonoBehaviour
{
    public PlayerStats playerStats;
    public float duration = 3.0f; // 장판 지속 시간
    public float damagePerSecond = 10.0f; // 초당 데미지
    public float speedReduction = 0.3f; // 이동 속도 감소 비율

    private List<MonsterController> affectedMonsters = new List<MonsterController>();
    private float timer = 0f;

    void Start()
    {
        // 장판이 깔리자마자 첫 데미지 적용
        DealDamageToMonsters();
        StartCoroutine(AreaEffect());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) // 몬스터 태그 확인
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null && !affectedMonsters.Contains(monster))
            {
                affectedMonsters.Add(monster);
                monster.moveSpeed *= (1 - speedReduction); // 이동 속도 감소
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null && affectedMonsters.Contains(monster))
            {
                affectedMonsters.Remove(monster);
                monster.moveSpeed /= (1 - speedReduction); // 원래 속도로 복구
            }
        }
    }

    private IEnumerator AreaEffect()
    {
        while (timer < duration)
        {
            yield return new WaitForSeconds(1.0f); // 1초 대기 후 데미지 적용
            DealDamageToMonsters(); // 매초 데미지 적용
            timer += 1.0f;
        }

        // 장판 종료 시 몬스터의 이동 속도 복구
        foreach (var monster in affectedMonsters)
        {
            if (monster != null)
            {
                monster.moveSpeed /= (1 - speedReduction);
            }
        }

        Destroy(gameObject); // 장판 오브젝트 삭제
    }

    // 몬스터들에게 데미지를 입히는 메서드
    private void DealDamageToMonsters()
    {
        foreach (var monster in affectedMonsters)
        {
            if (monster != null)
            {
                int calculatedDamage = (int)(playerStats.magicAttackDamage * playerStats.areaEffectDamageMultiplier);
                monster.TakeDamage(calculatedDamage); // 배율 적용
            }
        }
    }
}
