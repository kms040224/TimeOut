using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerController;

public class AttributeSelectionUI : MonoBehaviour
{
    // 속성 버튼들
    public Button fireButton;
    public Button lightningButton;

    // 속성 버튼 클릭 시 호출될 메서드
    private void Start()
    {
        fireButton.onClick.AddListener(() => SelectAttribute(AttributeType.Fire));
        lightningButton.onClick.AddListener(() => SelectAttribute(AttributeType.Lightning));
    }

    // 속성 선택 처리 메서드
    private void SelectAttribute(AttributeType attribute)
    {
        // PlayerAttribute가 씬에 있는지 확인
        if (PlayerAttribute.Instance != null)
        {
            PlayerAttribute.Instance.SetAttribute(attribute);  // 속성 설정
            SceneChanger.Instance.ChangeScene("Ch1_Stage_01");  // 씬 전환
        }
        else
        {
            Debug.LogError("PlayerAttribute instance is not found!");
        }
    }
}
