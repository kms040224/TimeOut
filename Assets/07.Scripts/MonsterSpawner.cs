using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab; // 몬스터 프리팹
    public Transform player; // 플레이어의 위치
    public float spawnInterval = 5.0f; // 몬스터 스폰 간격
    public Transform[] spawnPoints; // 스폰 포인트 배열
    public int monstersToSpawn = 3; // 한 번에 스폰할 몬스터 수

    private void Start()
    {
        InvokeRepeating(nameof(SpawnMonsters), 0f, spawnInterval);
    }

    void SpawnMonsters()
    {
        for (int i = 0; i < monstersToSpawn; i++)
        {
            // 랜덤 스폰 포인트 선택
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[spawnIndex];

            // 몬스터 생성
            GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);

            // 몬스터의 플레이어 변수를 설정
            MonsterController monsterController = monster.GetComponent<MonsterController>();
            if (monsterController != null)
            {
                monsterController.player = player;
            }
        }
    }
}