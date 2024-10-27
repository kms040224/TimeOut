using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnAreas; // 각 던전의 몬스터 스폰 구역
    public GameObject[] doors; // 던전을 연결하는 문들
    public GameObject[] monsterPrefabs; // 몬스터 프리팹
    public GameObject[] portals; // 던전별 포탈 오브젝트

    private int currentSpawnAreaIndex = 0; // 현재 던전 인덱스
    private int monsterCount; // 현재 던전의 몬스터 수

    private void Start()
    {
        // 첫 던전의 문을 열고 대기 (몬스터는 플레이어가 들어올 때 스폰)
        OpenCurrentArea();
    }

    // 특정 던전에 몬스터 스폰
    private void SpawnMonstersInArea(int areaIndex)
    {
        if (areaIndex >= spawnAreas.Length) return;

        Transform spawnArea = spawnAreas[areaIndex].transform;
        foreach (Transform spawnPoint in spawnArea)
        {
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            Instantiate(monsterPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        }

        monsterCount = spawnArea.childCount;
        Debug.Log($"던전 {areaIndex}에 {monsterCount} 마리의 몬스터가 스폰되었습니다.");
    }

    // 플레이어가 특정 던전에 들어왔을 때 호출되는 함수
    public void OnPlayerEnterArea(int areaIndex)
    {
        if (areaIndex == currentSpawnAreaIndex)
        {
            Debug.Log($"플레이어가 던전 {areaIndex}에 들어왔습니다.");
            SpawnMonstersInArea(areaIndex);
        }
    }

    // 몬스터가 죽을 때 호출되는 함수
    public void OnMonsterKilled()
    {
        monsterCount--;
        Debug.Log("남은 몬스터 수: " + monsterCount);

        if (monsterCount <= 0)
        {
            Debug.Log("현재 던전의 모든 몬스터가 처치되었습니다.");
            OpenNextArea();
            ActivatePortal(currentSpawnAreaIndex); // 현재 던전에 맞는 포탈 활성화
        }
    }

    // 현재 던전의 문을 열고 다음 던전의 문을 닫기
    private void OpenNextArea()
    {
        if (currentSpawnAreaIndex < doors.Length)
        {
            doors[currentSpawnAreaIndex].SetActive(false); // 현재 던전의 문 열기
        }

        currentSpawnAreaIndex++;
        if (currentSpawnAreaIndex < spawnAreas.Length)
        {
            OpenCurrentArea();
        }
        else
        {
            Debug.Log("모든 던전을 완료했습니다.");
        }
    }

    // 현재 던전의 문을 닫기
    private void OpenCurrentArea()
    {
        if (currentSpawnAreaIndex < doors.Length)
        {
            doors[currentSpawnAreaIndex].SetActive(true); // 다음 던전으로의 문 닫기
        }
    }

    // 포탈 활성화
    private void ActivatePortal(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < portals.Length)
        {
            portals[areaIndex].SetActive(true); // 해당 던전의 포탈 활성화
            Debug.Log($"던전 {areaIndex}에 포탈이 활성화되었습니다.");
        }
        else
        {
            Debug.LogWarning("유효하지 않은 areaIndex입니다.");
        }
    }

    // 문 상태를 변경하는 함수
    public void ToggleDoor(int index, bool isActive)
    {
        if (index >= 0 && index < doors.Length)
        {
            doors[index].SetActive(isActive);
            Debug.Log($"던전 {index}의 문이 {(isActive ? "활성화" : "비활성화")}되었습니다.");
        }
    }
}
