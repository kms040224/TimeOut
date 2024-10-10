using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public int damage = 20; // ���̾ ������
    public float lifetime = 5f; // ���̾�� �����Ǵ� �ð�
    public float speed = 10f; // ���̾ �ӵ�

    private Vector3 direction;

    void Start()
    {
        // ���� �ð��� ������ ���̾�� �ı�
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // ���̾�� �������� ���ư�����
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void Launch(Vector3 target)
    {
        // Ÿ�� ���� ����
        direction = (target - transform.position).normalized;
        direction.y = 0; // Y�� ���ŷ� ���� �̵�
    }

    // �浹 ����
    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� ������ ���
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                // ������ ü���� ����
                monster.TakeDamage(damage);
            }

            // ���̾�� �ı�
            Destroy(gameObject);
        }
    }
}
