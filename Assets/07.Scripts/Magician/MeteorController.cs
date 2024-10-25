using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour
{
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

    private void Update()
    {
        // ���׿��� ��ǥ �������� ��� �̵�
        transform.position += targetDirection * speed * Time.deltaTime;

        // ���׿��� �������� �Ÿ��� �浹 �˻�
        RaycastHit hit;
        if (Physics.Raycast(transform.position, targetDirection, out hit, speed * Time.deltaTime))
        {
            MonsterController monster = hit.collider.GetComponent<MonsterController>();
            if (monster != null && !damagedMonsters.Contains(monster))
            {
                monster.TakeDamage((int)damage); // ���Ϳ��� ������ ����
                damagedMonsters.Add(monster); // �̹� �������� ���� ���ͷ� �߰�
            }
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject); // �����ֱ� ���� �� ���׿� ������Ʈ ����
    }
}
