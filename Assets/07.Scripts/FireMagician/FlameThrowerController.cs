using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerController : MonoBehaviour
{
    public PlayerStats playerStats;
    public int damage = 10;  // 화염방사기로 주는 데미지

    void Start()
    {
        // 1.5초 후에 이 게임 오브젝트를 삭제
        Destroy(gameObject, 1.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                int calculatedDamage = (int)(playerStats.magicAttackDamage * playerStats.flamethrowerDamageMultiplier);
                monster.TakeDamage(calculatedDamage); // 배율 적용
            }
        }
    }
}
