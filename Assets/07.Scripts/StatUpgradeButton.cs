using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatUpgradeButton : MonoBehaviour
{
    public string statName;  // 업그레이드할 스탯 이름
    public float upgradeValue; // 업그레이드 값
    private Button button; // UI 버튼 컴포넌트

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            // 버튼 클릭 시 이벤트 연결
            button.onClick.AddListener(OnClick);
        }
        else
        {
            Debug.LogError($"{gameObject.name}에 Button 컴포넌트가 없습니다!");
        }
    }

    // 버튼 클릭 시 실행되는 메서드
    private void OnClick()
    {
        // StatUpgradeUI 인스턴스가 존재하는지 확인
        if (StatUpgradeUI.Instance != null)
        {
            // 스탯 업그레이드 실행
            StatUpgradeUI.Instance.UpgradeStat(statName, upgradeValue);
            StatUpgradeUI.Instance.HidePanel(); // 업그레이드 후 패널 숨기기
        }
        else
        {
            Debug.LogError("StatUpgradeUI.Instance가 null입니다!");
        }
    }

    // 버튼에 설정된 스탯과 업그레이드 값을 설정하는 메서드
    public void Setup(string stat, float value)
    {
        statName = stat;
        upgradeValue = value;
    }
}
