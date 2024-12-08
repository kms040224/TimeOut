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
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 속성을 설정하는 메서드
    public void SetAttribute(AttributeType attribute)
    {
        SelectedAttribute = attribute;
    }
}