using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnAreas; // 각 던전의 스폰 영역
    public GameObject[] doors; // 던전을 연결하는 문
    public GameObject[] portals; // 포탈 오브젝트

    private int currentSpawnAreaIndex = 0; // 현재 던전 인덱스
    private int currentWave = 0; // 현재 웨이브 (0: 첫 번째 웨이브, 1: 두 번째 웨이브)
    private int monsterCount = 0; // 현재 던전의 남은 몬스터 수
    public PlayerStats playerStats;
    private string selectedAttribute; // 선택된 속성 (Fire, Ice, etc.)

    private void Start()
    {
        OpenCurrentArea(); // 첫 던전의 문을 엽니다.
    }

    public PlayerStats GetPlayerStats()
    {
        return playerStats;
    }
    // 속성 설정 함수
    public void SetSelectedAttribute(string attribute)
    {
        selectedAttribute = attribute;
        Debug.Log($"선택된 속성: {selectedAttribute}");
    }

    // 선택된 속성을 반환
    public string GetSelectedAttribute()
    {
        return selectedAttribute;
    }

    // 특정 웨이브에서 몬스터 스폰
    private void SpawnMonstersInWave(int areaIndex, int waveIndex)
    {
        if (areaIndex >= spawnAreas.Length || spawnAreas[areaIndex] == null)
        {
            Debug.LogWarning($"spawnAreas[{areaIndex}]가 null이거나 유효하지 않습니다.");
            return;
        }

        Transform spawnArea = spawnAreas[areaIndex].transform;
        Transform wave = spawnArea.Find($"Wave{waveIndex + 1}");

        if (wave == null)
        {
            Debug.Log($"Wave{waveIndex + 1}가 던전 {areaIndex}에 없습니다.");
            OnAllWavesCleared(); // 다음 웨이브가 없으면 모든 웨이브 완료 처리
            return;
        }

        foreach (Transform spawnPoint in wave)
        {
            SpawnPoint spawnPointScript = spawnPoint.GetComponent<SpawnPoint>();

            if (spawnPointScript != null && spawnPointScript.assignedMonster != null)
            {
                Instantiate(spawnPointScript.assignedMonster, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"스폰 포인트 {spawnPoint.name}에 할당된 몬스터가 없습니다.");
            }
        }

        monsterCount = wave.childCount;
        Debug.Log($"던전 {areaIndex}의 Wave{waveIndex + 1}에 {monsterCount} 마리의 몬스터가 스폰되었습니다.");
    }

    // 플레이어가 특정 던전에 들어왔을 때 호출되는 함수
    public void OnPlayerEnterArea(int areaIndex)
    {
        if (areaIndex < 0 || areaIndex >= spawnAreas.Length) // 범위 체크
        {
            Debug.LogError($"유효하지 않은 areaIndex: {areaIndex}");
            return;
        }

        if (areaIndex == currentSpawnAreaIndex)
        {
            Debug.Log($"플레이어가 던전 {areaIndex}에 들어왔습니다.");
            SpawnMonstersInWave(areaIndex, currentWave); // 첫 번째 웨이브 스폰
        }
    }

    // 몬스터가 죽을 때 호출되는 함수
    public void OnMonsterKilled()
    {
        monsterCount--;
        Debug.Log("남은 몬스터 수: " + monsterCount);

        if (monsterCount <= 0)
        {
            Debug.Log($"Wave{currentWave + 1}의 모든 몬스터가 처치되었습니다.");
            currentWave++;

            SpawnMonstersInWave(currentSpawnAreaIndex, currentWave); // 다음 웨이브 스폰
        }
    }

    // 모든 웨이브가 끝났을 때 호출
    private void OnAllWavesCleared()
    {
        Debug.Log("현재 던전의 모든 웨이브가 완료되었습니다.");

        OpenNextArea();
        ActivatePortal(currentSpawnAreaIndex); // 포탈 활성화

        // StatUpgradeUI 인스턴스 확인 및 활성화
        if (StatUpgradeUI.Instance == null)
        {
            GameObject uiObject = GameObject.Find("StatUpgradeUI"); // 오브젝트 이름으로 찾기
            if (uiObject != null)
            {
                uiObject.SetActive(true); // 활성화
                StatUpgradeUI.Instance = uiObject.GetComponent<StatUpgradeUI>(); // 참조 설정
            }
            else
            {
                Debug.LogWarning("StatUpgradeUI 오브젝트가 씬에 없습니다.");
            }
        }

        if (StatUpgradeUI.Instance != null)
        {
            StatUpgradeUI.Instance.ShowPanel();
        }
        else
        {
            Debug.LogError("StatUpgradeUI.Instance가 null입니다. StatUpgradeUI가 씬에 추가되었는지 확인하세요.");
        }
    }

    // 현재 던전의 문을 열고 다음 던전의 문을 닫기
    private void OpenNextArea()
    {
        if (currentSpawnAreaIndex < doors.Length && doors[currentSpawnAreaIndex] != null)
        {
            doors[currentSpawnAreaIndex].SetActive(false); // 현재 던전의 문 열기
        }

        currentSpawnAreaIndex++;
        currentWave = 0; // 웨이브를 초기화
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
        if (currentSpawnAreaIndex < doors.Length && doors[currentSpawnAreaIndex] != null)
        {
            doors[currentSpawnAreaIndex].SetActive(true); // 다음 던전으로의 문 닫기
        }
    }

    // 포탈 활성화
    private void ActivatePortal(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < portals.Length && portals[areaIndex] != null)
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
        if (index >= 0 && index < doors.Length && doors[index] != null)
        {
            doors[index].SetActive(isActive);
            Debug.Log($"던전 {index}의 문이 {(isActive ? "활성화" : "비활성화")}되었습니다.");
        }
        else
        {
            Debug.LogWarning($"유효하지 않은 index: {index}. 해당 던전의 문을 찾을 수 없습니다.");
        }
    }
}
