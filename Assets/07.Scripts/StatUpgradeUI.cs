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
    public Transform[] buttonPositions; // ��ư ��ġ ��ġ �迭

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

        SetupButtons(); // ��ư �ʱ�ȭ
    }

    // ��ư ���� �ʱ�ȭ
    private void SetupButtons()
    {
        // �� ��ư�� ���׷��̵� ���� ����
        statButtons[0].Setup("movementSpeed", 1f);
        statButtons[1].Setup("magicAttackDamage", 5f);
        statButtons[2].Setup("flamethrowerCooldown", -1f);

        // �ʿ� �� �߰� ��ư ���� ����
    }

    // �������� 3���� ��ư�� ǥ��
    public void ShowPanel()
    {
        if (statUpgradePanel == null)
        {
            Debug.LogWarning("StatUpgradePanel is not assigned!");
            return;
        }

        // �г� Ȱ��ȭ
        statUpgradePanel.SetActive(true);

        // ��� ��ư ��Ȱ��ȭ
        foreach (var button in statButtons)
        {
            button.gameObject.SetActive(false);
        }

        // �����ϰ� 3�� ��ư ����
        List<int> randomIndices = GetUniqueRandomIndices(3, statButtons.Length);

        for (int i = 0; i < randomIndices.Count; i++)
        {
            int index = randomIndices[i];
            StatUpgradeButton button = statButtons[index];

            // ��ư Ȱ��ȭ �� ��ġ
            button.gameObject.SetActive(true);
            button.transform.position = buttonPositions[i].position;
        }

        Debug.Log("Stat Upgrade Panel opened with random 3 buttons.");
    }

    // �г� �ݱ�
    public void HidePanel()
    {
        if (statUpgradePanel != null)
        {
            statUpgradePanel.SetActive(false);
            Debug.Log("Stat Upgrade Panel closed.");
        }
    }

    // ������ ���� �ε��� ����
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