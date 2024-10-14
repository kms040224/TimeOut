using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public int areaIndex; // 구간 번호
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 트리거에 들어오면
        {
            gameManager.OnPlayerEnterArea(areaIndex); // 해당 구간의 몬스터 스폰

            // 이전 구간의 문을 다시 활성화
            if (areaIndex > 0) // 첫 번째 구역이 아닐 경우
            {
                gameManager.ToggleDoor(areaIndex - 1, true); // 이전 문을 활성화
            }
        }
    }
}