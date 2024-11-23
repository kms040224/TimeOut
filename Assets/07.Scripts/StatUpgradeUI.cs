using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUpgradeUI : MonoBehaviour
{
    public static StatUpgradeUI Instance; // 싱글톤 인스턴스
    public GameObject statUpgradePanel; // 스탯 업그레이드 패널 오브젝트
    public PlayerStats playerStats; // ScriptableObject 참조
    public StatUpgradeButton[] statButtons; // 모든 버튼 배열
    public Transform[] buttonPositions; // 버튼 배치 위치 배열

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is not assigned!");
        }

        SetupButtons(); // 버튼 초기화
    }

    // 버튼 정보 초기화
    private void SetupButtons()
    {
        // 각 버튼에 업그레이드 정보 설정
        statButtons[0].Setup("movementSpeed", 1f);
        statButtons[1].Setup("magicAttackDamage", 5f);
        statButtons[2].Setup("flamethrowerCooldown", -1f);

        // 필요 시 추가 버튼 설정 가능
    }

    // 랜덤으로 3개의 버튼을 표시
    public void ShowPanel()
    {
        if (statUpgradePanel == null)
        {
            Debug.LogWarning("StatUpgradePanel is not assigned!");
            return;
        }

        // 패널 활성화
        statUpgradePanel.SetActive(true);

        // 모든 버튼 비활성화
        foreach (var button in statButtons)
        {
            button.gameObject.SetActive(false);
        }

        // 랜덤하게 3개 버튼 선택
        List<int> randomIndices = GetUniqueRandomIndices(3, statButtons.Length);

        for (int i = 0; i < randomIndices.Count; i++)
        {
            int index = randomIndices[i];
            StatUpgradeButton button = statButtons[index];

            // 버튼 활성화 및 배치
            button.gameObject.SetActive(true);
            button.transform.position = buttonPositions[i].position;
        }

        Debug.Log("Stat Upgrade Panel opened with random 3 buttons.");
    }

    // 패널 닫기
    public void HidePanel()
    {
        if (statUpgradePanel != null)
        {
            statUpgradePanel.SetActive(false);
            Debug.Log("Stat Upgrade Panel closed.");
        }
    }

    // 고유한 랜덤 인덱스 생성
    private List<int> GetUniqueRandomIndices(int count, int max)
    {
        List<int> indices = new List<int>();
        while (indices.Count < count)
        {
            int randomIndex = Random.Range(0, max);
            if (!indices.Contains(randomIndex))
            {
                indices.Add(randomIndex);
            }
        }
        return indices;
    }

    public void UpgradeStat(string statName, float upgradeValue)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null! Cannot upgrade stats.");
            return;
        }

        PlayerStats playerStats = GameManager.Instance.GetPlayerStats();
        if (playerStats == null)
        {
            Debug.LogError("PlayerStats is null! Cannot upgrade stats.");
            return;
        }

        playerStats.UpgradeStat(statName, upgradeValue);
        Debug.Log($"Stat {statName} upgraded by {upgradeValue}.");
    }
}