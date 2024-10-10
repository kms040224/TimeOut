using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // ���� ������
    public Transform[] spawnPoints; // ���� ����Ʈ �迭
    public int monstersToSpawn = 3; // �� ���� ������ ���� ��

    private void Start()
    {
        // ���� ���� �� �� ���� ���� ����
        SpawnMonsters();
    }

    void SpawnMonsters()
    {
        for (int i = 0; i < monstersToSpawn; i++)
        {
            // ���� ���� ����Ʈ ����
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // ���� ����
            Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
