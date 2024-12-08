using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PlayerController;

public class AttributeSelectionUI : MonoBehaviour
{
    // �Ӽ� ��ư��
    public Button fireButton;
    public Button lightningButton;

    // �Ӽ� ��ư Ŭ�� �� ȣ��� �޼���
    private void Start()
    {
        fireButton.onClick.AddListener(() => SelectAttribute(AttributeType.Fire));
        lightningButton.onClick.AddListener(() => SelectAttribute(AttributeType.Lightning));
    }

    // �Ӽ� ���� ó�� �޼���
    private void SelectAttribute(AttributeType attribute)
    {
        // PlayerAttribute�� ���� �ִ��� Ȯ��
        if (PlayerAttribute.Instance != null)
        {
            PlayerAttribute.Instance.SetAttribute(attribute);  // �Ӽ� ����
            SceneChanger.Instance.ChangeScene("Ch1_Stage_01");  // �� ��ȯ
        }
        else
        {
            Debug.LogError("PlayerAttribute instance is not found!");
        }
    }
}
