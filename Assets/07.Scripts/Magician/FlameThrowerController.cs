using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlamethrowerController : MonoBehaviour
{
    public int damage = 10;  // ȭ������� �ִ� ������

    void Start()
    {
        // 1.5�� �Ŀ� �� ���� ������Ʈ�� ����
        Destroy(gameObject, 1.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
            }
        }
    }
}
