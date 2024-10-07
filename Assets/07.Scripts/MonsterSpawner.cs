using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // ���� ������
    public Transform player; // �÷��̾��� ��ġ
    public float spawnInterval = 5.0f; // ���� ���� ����
    public Transform[] spawnPoints; // ���� ����Ʈ �迭
    public int monstersToSpawn = 3; // �� ���� ������ ���� ��

    private void Start()
    {
        InvokeRepeating(nameof(SpawnMonsters), 0f, spawnInterval);
    }

    void SpawnMonsters()
    {
        for (int i = 0; i < monstersToSpawn; i++)
        {
            // ���� ���� ����Ʈ ����
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // ���� ����
            GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);

            // ������ �÷��̾� ������ ����
            MonsterController monsterController = monster.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                monsterController.player = player;
            }
        }
    }
}