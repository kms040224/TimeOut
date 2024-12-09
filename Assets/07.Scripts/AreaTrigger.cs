using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public int areaIndex; // 구간 번호
    private GameManager gameManager;
    private Collider areaCollider; // 콜라이더 참조

    void Start()
    {
        // 씬에서 GameManager를 찾아서 할당
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다!");
            return; // 게임을 진행할 수 없으므로 종료
        }

        // 콜라이더 컴포넌트 가져오기
        areaCollider = GetComponent<Collider>();
        if (areaCollider == null)
        {
            Debug.LogError("Collider가 AreaTrigger 오브젝트에 없습니다!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (areaCollider == null)
        {
            Debug.LogError($"AreaCollider가 null입니다. AreaTrigger {areaIndex}를 확인하세요.");
            return;
        }

        Debug.Log($"Collider {other.name} entered AreaTrigger {areaIndex}");

        // 플레이어가 트리거에 들어왔을 때 처리
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player entered Area {areaIndex}");
            gameManager.OnPlayerEnterArea(areaIndex);

            // 이전 구간의 문 활성화
            if (areaIndex > 0)
            {
                gameManager.ToggleDoor(areaIndex - 1, true);
                Debug.Log($"Door for area {areaIndex - 1} has been enabled.");
            }

            // 트리거 콜라이더 비활성화
            areaCollider.enabled = false;
            Debug.Log($"Area {areaIndex} trigger disabled.");
        }
        else
        {
            Debug.Log($"Non-player object {other.name} entered AreaTrigger {areaIndex}");
        }
    }
}
