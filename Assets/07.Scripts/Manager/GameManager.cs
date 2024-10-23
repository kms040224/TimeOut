using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnAreas; // �� ������ ���� ���� ���� ����
    public GameObject[] doors; // ������ �����ϴ� ���� (�� ������ ���Թ�)
    public GameObject[] monsterPrefabs; // ���� ������
    public GameObject portalPrefab; // ��Ż ������
    public Transform portalSpawnPoint; // ��Ż�� ������ ��ġ

    private int currentSpawnAreaIndex = 0; // ���� ���� �ε���
    private int monsterCount; // ���� ������ ���� ��
    private bool isPortalSpawned = false; // ��Ż�� �����Ǿ����� Ȯ��

    private void Start()
    {
        // ù ������ ���� ���� ��� (���ʹ� �÷��̾ ���� �� ����)
        OpenCurrentArea();
    }

    // Ư�� ������ ���� ����
    private void SpawnMonstersInArea(int areaIndex)
    {
        Transform spawnArea = spawnAreas[areaIndex].transform;
        foreach (Transform spawnPoint in spawnArea)
        {
            // �� ���� ����Ʈ�� ���� ��ȯ
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            Instantiate(monsterPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        }

        // ������ ���� �� ���
        monsterCount = spawnArea.childCount;
        Debug.Log($"���� {areaIndex}�� {monsterCount} ������ ���Ͱ� �����Ǿ����ϴ�.");
    }

    // �÷��̾ Ư�� ������ ������ �� ȣ��Ǵ� �Լ�
    public void OnPlayerEnterArea(int areaIndex)
    {
        if (areaIndex == currentSpawnAreaIndex) // �÷��̾ ���� ������ ������ ���� ����
        {
            Debug.Log($"�÷��̾ ���� {areaIndex}�� ���Խ��ϴ�.");
            SpawnMonstersInArea(areaIndex);
        }
    }

    // ���Ͱ� ���� �� ȣ��Ǵ� �Լ�
    public void OnMonsterKilled()
    {
        monsterCount--;
        Debug.Log("���� ���� ��: " + monsterCount);

        // ������ ��� ���Ͱ� �׾����� Ȯ��
        if (monsterCount <= 0)
        {
            Debug.Log("���� ������ ��� ���Ͱ� óġ�Ǿ����ϴ�.");
            OpenNextArea();
            SpawnPortal(); // ��Ż ����
        }
    }

    // ���� ������ ���� ���� ���� ������ ���� �ݱ�
    private void OpenNextArea()
    {
        if (currentSpawnAreaIndex < doors.Length)
        {
            doors[currentSpawnAreaIndex].SetActive(false); // ���� ������ �� ����
        }

        currentSpawnAreaIndex++;
        if (currentSpawnAreaIndex < spawnAreas.Length)
        {
            OpenCurrentArea();
        }
        else
        {
            Debug.Log("��� ������ �Ϸ��߽��ϴ�.");
        }
    }

    // ���� ������ ���� �ݱ�
    private void OpenCurrentArea()
    {
        if (currentSpawnAreaIndex < doors.Length)
        {
            doors[currentSpawnAreaIndex].SetActive(true); // ���� ���������� �� �ݱ�
        }
    }

    // ��Ż ����
    private void SpawnPortal()
    {
        if (!isPortalSpawned && portalPrefab != null && portalSpawnPoint != null)
        {
            Instantiate(portalPrefab, portalSpawnPoint.position, Quaternion.identity);
            isPortalSpawned = true; // ��Ż�� �� ���� �����ǵ��� ����
            Debug.Log("��Ż�� �����Ǿ����ϴ�.");
        }
    }

    public void ToggleDoor(int index, bool isActive)
    {
        if (index >= 0 && index < doors.Length)
        {
            doors[index].SetActive(isActive);
            Debug.Log($"���� {index}�� ���� {(isActive ? "Ȱ��ȭ" : "��Ȱ��ȭ")}�Ǿ����ϴ�.");
        }
    }
}
