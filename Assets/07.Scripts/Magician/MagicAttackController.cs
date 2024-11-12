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
    private float lifetimeTimer;

    void OnEnable()
    {
        // ������Ʈ�� Ȱ��ȭ�� ������ lifetime Ÿ�̸� �ʱ�ȭ
        lifetimeTimer = lifetime;
    }

    void Update()
    {
        // ���̾�� �������� ���ư�����
        transform.Translate(direction * speed * Time.deltaTime);

        // ������ ���ϸ� ������Ʈ Ǯ�� ��ȯ
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            ReturnToPool();
        }
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
        // ���� �Ǵ� ������ ���
        if (other.CompareTag("Monster"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            BossController boss = other.GetComponent<BossController>();

            if (monster != null)
            {
                // ������ ü�� ����
                monster.TakeDamage(damage);
            }
            else if (boss != null)
            {
                // ������ ü�� ����
                boss.TakeDamage(damage);
            }

            // ��Ʈ ����Ʈ ����
            if (hitEffectPrefab != null)
            {
                GameObject hitEffect = Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                Destroy(hitEffect, 1f);
            }

            // ���̾�� Ǯ�� ��ȯ
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        // ������Ʈ Ǯ�� ��ȯ
        ObjectPool.Instance.ReturnMagicAttack(gameObject);
    }
}
