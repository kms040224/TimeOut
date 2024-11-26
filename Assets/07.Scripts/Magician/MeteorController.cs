using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
    public PlayerStats playerStats;
    public float speed = 15f; // ���׿� �̵� �ӵ�
    public float damage = 50f; // ���׿� ������
    public float lifetime = 5f; // ���׿� �����ֱ�
    private Vector3 targetDirection;

    private HashSet<MonsterController> damagedMonsters = new HashSet<MonsterController>(); // �������� ���� ���� ����Ʈ

    public void Launch(Vector3 target)
    {
        // ��ǥ ������ ����
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
                monster.TakeDamage(calculatedDamage); // ���� ����
                damagedMonsters.Add(monster);
            }
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject); // �����ֱ� ���� �� ���׿� ������Ʈ ����
    }
}
