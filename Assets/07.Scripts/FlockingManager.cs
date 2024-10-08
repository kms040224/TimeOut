using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public List<MonsterController> monsters = new List<MonsterController>(); // ���� ����Ʈ

    // ���͸� ����ϴ� �޼���
    public void RegisterMonster(MonsterController monster)
    {
        if (!monsters.Contains(monster))
        {
            monsters.Add(monster);
        }
    }

    // ���͸� ����Ʈ���� �����ϴ� �޼���
    public void UnregisterMonster(MonsterController monster)
    {
        if (monsters.Contains(monster))
        {
            monsters.Remove(monster);
        }
    }

    void Update()
    {
        // �� ���Ϳ� ���� �÷�ŷ ������Ʈ ȣ��
        foreach (var monster in monsters)
        {
            monster.FlockingBehavior(); // ������ �÷�ŷ �ൿ ������Ʈ
        }
    }
}