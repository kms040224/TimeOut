using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUpgradeButton : MonoBehaviour
{
    public string statName;  // ���׷��̵��� ���� �̸�
    public float upgradeValue; // ���׷��̵� ��
    private Button button; // UI ��ư ������Ʈ

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            // ��ư Ŭ�� �� �̺�Ʈ ����
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError($"{gameObject.name}�� Button ������Ʈ�� �����ϴ�!");
        }
    }

    // ��ư Ŭ�� �� ����Ǵ� �޼���
    private void OnClick()
    {
        // StatUpgradeUI �ν��Ͻ��� �����ϴ��� Ȯ��
        if (StatUpgradeUI.Instance != null)
        {
            // ���� ���׷��̵� ����
            StatUpgradeUI.Instance.UpgradeStat(statName, upgradeValue);
            StatUpgradeUI.Instance.HidePanel(); // ���׷��̵� �� �г� �����
        }
        else
        {
            Debug.LogError("StatUpgradeUI.Instance�� null�Դϴ�!");
        }
    }

    // ��ư�� ������ ���Ȱ� ���׷��̵� ���� �����ϴ� �޼���
    public void Setup(string stat, float value)
    {
        statName = stat;
        upgradeValue = value;
    }
}
