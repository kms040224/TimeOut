using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;
public class PlayerAttribute : MonoBehaviour
{
    public static PlayerAttribute Instance { get; private set; }

    public AttributeType SelectedAttribute { get; private set; }

    private void Awake()
    {
        // �̱��� ���� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� ����
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �Ӽ��� �����ϴ� �޼���
    public void SetAttribute(AttributeType attribute)
    {
        SelectedAttribute = attribute;
    }
}