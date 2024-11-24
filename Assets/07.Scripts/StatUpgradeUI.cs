using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUpgradeUI : MonoBehaviour
{
    public static StatUpgradeUI Instance; // �̱��� �ν��Ͻ�
    public GameObject statUpgradePanel; // ���� ���׷��̵� �г� ������Ʈ
    public PlayerStats playerStats; // ScriptableObject ����
    public StatUpgradeButton[] statButtons; // ��� ��ư �迭
    public RectTransform[] buttonPositions; // ��ư ��ġ ��ġ �迭

    private void Awake()
    {
        // �̱��� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // �г��� Ȱ��ȭ�ϴ� �Լ�
    public void ShowPanel()
    {
        if (statUpgradePanel == null)
        {
            Debug.LogWarning("StatUpgradePanel is not assigned!");
            return;
        }

        statUpgradePanel.SetActive(true); // �г� Ȱ��ȭ
        Debug.Log("Stat Upgrade Panel is now active.");

        ActivateRandomButtons(); // �������� ��ư Ȱ��ȭ
    }

    // �г��� ����� �Լ�
    public void HidePanel()
    {
        if (statUpgradePanel != null)
        {
            statUpgradePanel.SetActive(false);
            Debug.Log("Stat Upgrade Panel is now hidden.");
        }
    }

    // ������ ���� �ε����� �����ϴ� �Լ�
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

    // ���� ��ư 3�� Ȱ��ȭ
    public void ActivateRandomButtons()
    {
        // ���� �ε����� 3�� ���
        List<int> randomIndices = GetUniqueRandomIndices(3, statButtons.Length);

        // ��� ��ư�� ��Ȱ��ȭ
        foreach (var button in statButtons)
        {
            button.gameObject.SetActive(false); // ��� ��ư�� ��Ȱ��ȭ
        }

        // 3���� ������ ��ġ�� �������� ��ư ��ġ
        for (int i = 0; i < 3; i++)
        {
            var selectedButton = statButtons[randomIndices[i]];

            // ���õ� ��ư�� Ȱ��ȭ
            selectedButton.gameObject.SetActive(true);

            // RectTransform�� ����� UI ��ġ ���� (��Ŀ�� �θ� �°� ����)
            RectTransform rectTransform = selectedButton.GetComponent<RectTransform>();

            // ������ ��ġ�� ��ư ��ġ (���⼭�� ��ư ��ġ�� RectTransform���� ����)
            rectTransform.anchoredPosition = buttonPositions[i].anchoredPosition;
        }
    }
    // ������ ���׷��̵��ϴ� �Լ�
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