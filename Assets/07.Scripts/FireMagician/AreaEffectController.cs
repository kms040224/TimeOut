using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffectController : MonoBehaviour
{
    public PlayerStats playerStats;
    public float duration = 3.0f; // ���� ���� �ð�
    public float damagePerSecond = 10.0f; // �ʴ� ������
    public float speedReduction = 0.3f; // �̵� �ӵ� ���� ����

    private List<MonsterController> affectedMonsters = new List<MonsterController>();
    private float timer = 0f;

    void Start()
    {
        // ������ ���ڸ��� ù ������ ����
        DealDamageToMonsters();
        StartCoroutine(AreaEffect());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster")) // ���� �±� Ȯ��
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null && !affectedMonsters.Contains(monster))
            {
                affectedMonsters.Add(monster);
                monster.moveSpeed *= (1 - speedReduction); // �̵� �ӵ� ����
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
                monster.moveSpeed /= (1 - speedReduction); // ���� �ӵ��� ����
            }
        }
    }

    private IEnumerator AreaEffect()
    {
        while (timer < duration)
        {
            yield return new WaitForSeconds(1.0f); // 1�� ��� �� ������ ����
            DealDamageToMonsters(); // ���� ������ ����
            timer += 1.0f;
        }

        // ���� ���� �� ������ �̵� �ӵ� ����
        foreach (var monster in affectedMonsters)
        {
            if (monster != null)
            {
                monster.moveSpeed /= (1 - speedReduction);
            }
        }

        Destroy(gameObject); // ���� ������Ʈ ����
    }

    // ���͵鿡�� �������� ������ �޼���
    private void DealDamageToMonsters()
    {
        foreach (var monster in affectedMonsters)
        {
            if (monster != null)
            {
                int calculatedDamage = (int)(playerStats.magicAttackDamage * playerStats.areaEffectDamageMultiplier);
                monster.TakeDamage(calculatedDamage); // ���� ����
            }
        }
    }
}
