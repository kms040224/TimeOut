using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public List<MonsterController> monsters = new List<MonsterController>(); // 몬스터 리스트

    // 몬스터를 등록하는 메서드
    public void RegisterMonster(MonsterController monster)
    {
        if (!monsters.Contains(monster))
        {
            monsters.Add(monster);
        }
    }

    // 몬스터를 리스트에서 제거하는 메서드
    public void UnregisterMonster(MonsterController monster)
    {
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }
    }

    void Update()
    {
        // 각 몬스터에 대해 플로킹 업데이트 호출
        foreach (var monster in monsters)
        {
            monster.FlockingBehavior(); // 몬스터의 플로킹 행동 업데이트
        }
    }
}