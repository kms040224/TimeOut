using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnAreas; // 각 구간에 대한 몬스터 스폰 구역
    public GameObject[] doors; // 구간을 연결하는 문들 (각 구간의 출입문)
    public GameObject[] monsterPrefabs; // 몬스터 프리팹
    public GameObject portalPrefab; // 포탈 프리팹
    public Transform portalSpawnPoint; // 포탈이 스폰될 위치

    private int currentSpawnAreaIndex = 0; // 현재 구간 인덱스
    private int monsterCount; // 현재 구간의 몬스터 수
    private bool isPortalSpawned = false; // 포탈이 스폰되었는지 확인

    private void Start()
    {
        // 첫 구간의 문을 열고 대기 (몬스터는 플레이어가 들어올 때 스폰)
        OpenCurrentArea();
    }

    // 특정 구간에 몬스터 스폰
    private void SpawnMonstersInArea(int areaIndex)
    {
        Transform spawnArea = spawnAreas[areaIndex].transform;
        foreach (Transform spawnPoint in spawnArea)
        {
            // 각 스폰 포인트에 몬스터 소환
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            Instantiate(monsterPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        }

        // 스폰된 몬스터 수 계산
        monsterCount = spawnArea.childCount;
        Debug.Log($"구역 {areaIndex}에 {monsterCount} 마리의 몬스터가 스폰되었습니다.");
    }

    // 플레이어가 특정 구역에 들어왔을 때 호출되는 함수
    public void OnPlayerEnterArea(int areaIndex)
    {
        if (areaIndex == currentSpawnAreaIndex) // 플레이어가 현재 구역에 들어왔을 때만 스폰
        {
            Debug.Log($"플레이어가 구역 {areaIndex}에 들어왔습니다.");
            SpawnMonstersInArea(areaIndex);
        }
    }

    // 몬스터가 죽을 때 호출되는 함수
    public void OnMonsterKilled()
    {
        monsterCount--;
        Debug.Log("남은 몬스터 수: " + monsterCount);

        // 구역의 모든 몬스터가 죽었는지 확인
        if (monsterCount <= 0)
        {
            Debug.Log("현재 구역의 모든 몬스터가 처치되었습니다.");
            OpenNextArea();
            SpawnPortal(); // 포탈 생성
        }
    }

    // 현재 구간의 문을 열고 다음 구간의 문을 닫기
    private void OpenNextArea()
    {
        if (currentSpawnAreaIndex < doors.Length)
        {
            doors[currentSpawnAreaIndex].SetActive(false); // 현재 구간의 문 열기
        }

        currentSpawnAreaIndex++;
        if (currentSpawnAreaIndex < spawnAreas.Length)
        {
            OpenCurrentArea();
        }
        else
        {
            Debug.Log("모든 구간을 완료했습니다.");
        }
    }

    // 현재 구간의 문을 닫기
    private void OpenCurrentArea()
    {
        if (currentSpawnAreaIndex < doors.Length)
        {
            doors[currentSpawnAreaIndex].SetActive(true); // 다음 구간으로의 문 닫기
        }
    }

    // 포탈 생성
    private void SpawnPortal()
    {
        if (!isPortalSpawned && portalPrefab != null && portalSpawnPoint != null)
        {
            Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);
            isPortalSpawned = true; // 포탈이 한 번만 생성되도록 설정
            Debug.Log("포탈이 생성되었습니다.");
        }
    }

    public void ToggleDoor(int index, bool isActive)
    {
        if (index >= 0 && index < doors.Length)
        {
            doors[index].SetActive(isActive);
            Debug.Log($"구역 {index}의 문이 {(isActive ? "활성화" : "비활성화")}되었습니다.");
        }
    }
}
