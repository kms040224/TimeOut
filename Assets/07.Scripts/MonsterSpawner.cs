using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public Transform[] spawnPoints; // 스폰 포인트 배열
    public int monstersToSpawn = 3; // 한 번에 스폰할 몬스터 수

    private void Start()
    {
        // 게임 시작 시 한 번만 몬스터 스폰
        SpawnMonsters();
    }

    void SpawnMonsters()
    {
        for (int i = 0; i < monstersToSpawn; i++)
        {
            // 랜덤 스폰 포인트 선택
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // 몬스터 생성
            Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
