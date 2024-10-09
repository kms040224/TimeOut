using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject portalPrefab; // 포탈 프리팹
    public Transform portalSpawnPoint; // 포탈이 생성될 위치
    private GameObject[] monsters;

    void Start()
    {
        // 몬스터 태그를 가진 모든 오브젝트를 찾아 배열에 저장
        monsters = GameObject.FindGameObjectsWithTag("Monster");
    }

    void Update()
    {
        CheckMonsters();
    }

    // 모든 몬스터가 죽었는지 확인하는 함수
    void CheckMonsters()
    {
        foreach (GameObject monster in monsters)
        {
            if (monster != null) return; // 하나라도 살아있으면 함수 종료
        }

        // 모든 몬스터가 죽었으면 포탈 생성
        SpawnPortal();
    }

    // 포탈을 생성하는 함수
    void SpawnPortal()
    {
        // 포탈이 이미 생성되었는지 확인
        if (portalPrefab != null && !portalPrefab.activeSelf)
        {
            portalPrefab.SetActive(true); // 포탈 활성화
            portalPrefab.transform.position = portalSpawnPoint.position; // 포탈 위치 설정
        }
    }
}