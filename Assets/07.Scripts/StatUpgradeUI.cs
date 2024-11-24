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
    public RectTransform[] buttonPositions; // 버튼 배치 위치 배열

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 패널을 활성화하는 함수
    public void ShowPanel()
    {
        if (statUpgradePanel == null)
        {
            Debug.LogWarning("StatUpgradePanel is not assigned!");
            return;
        }

        statUpgradePanel.SetActive(true); // 패널 활성화
        Debug.Log("Stat Upgrade Panel is now active.");

        ActivateRandomButtons(); // 랜덤으로 버튼 활성화
    }

    // 패널을 숨기는 함수
    public void HidePanel()
    {
        if (statUpgradePanel != null)
        {
            statUpgradePanel.SetActive(false);
            Debug.Log("Stat Upgrade Panel is now hidden.");
        }
    }

    // 고유한 랜덤 인덱스를 생성하는 함수
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

    // 랜덤 버튼 3개 활성화
    public void ActivateRandomButtons()
    {
        // 랜덤 인덱스를 3개 얻기
        List<int> randomIndices = GetUniqueRandomIndices(3, statButtons.Length);

        // 모든 버튼을 비활성화
        foreach (var button in statButtons)
        {
            button.gameObject.SetActive(false); // 모든 버튼을 비활성화
        }

        // 3개의 고정된 위치에 랜덤으로 버튼 배치
        for (int i = 0; i < 3; i++)
        {
            var selectedButton = statButtons[randomIndices[i]];

            // 선택된 버튼을 활성화
            selectedButton.gameObject.SetActive(true);

            // RectTransform을 사용해 UI 위치 설정 (앵커를 부모에 맞게 설정)
            RectTransform rectTransform = selectedButton.GetComponent<RectTransform>();

            // 고정된 위치에 버튼 배치 (여기서는 버튼 위치를 RectTransform으로 설정)
            rectTransform.anchoredPosition = buttonPositions[i].anchoredPosition;
        }
    }
    // 스탯을 업그레이드하는 함수
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