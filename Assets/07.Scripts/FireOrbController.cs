using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireOrbController : MonoBehaviour
{
    public float speed = 5f;  // ������ �̵� �ӵ�
    public int damage = 20;   // ���갡 ���Ϳ��� ������ ������
    private Transform target; // ���� ����� ���� Ÿ��

    // ���� ����� ���� ����
    public void SetTarget(Transform monsterTarget)
    {
        target = monsterTarget;
    }

    void Update()
    {
        // Ÿ���� ������ ���� �ı�
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Ÿ���� ���� �̵�
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    // ���Ϳ� �浹 �� ������ ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();

            if (monster != null)
            {
                monster.TakeDamage(damage); // ���Ϳ��� ������ ����
                Debug.Log("FireOrb hit the monster, dealing " + damage + " damage.");
                Destroy(gameObject); // �浹 �� ���̾� ���� ����
            }
        }
    }
}
