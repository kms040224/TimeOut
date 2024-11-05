using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttackController : MonoBehaviour
{
    public int damage = 20; // ���̾ ������
    public float lifetime = 5f; // ���̾�� �����Ǵ� �ð�
    public float speed = 10f; // ���̾ �ӵ�
    public GameObject hitEffectPrefab; // Ÿ�� �� ������ ����Ʈ ������

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

                // ��Ʈ ����Ʈ ����
                if (hitEffectPrefab != null)
                {
                    // ��Ʈ ����Ʈ�� ������ ����
                    GameObject hitEffect = Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);

                    // 1�� �Ŀ� ��Ʈ ����Ʈ�� ������ �ı�
                    Destroy(hitEffect, 1f);
                }
            }

            // ���̾�� �ı�
            Destroy(gameObject);
        }
    }
}