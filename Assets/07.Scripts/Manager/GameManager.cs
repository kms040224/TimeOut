using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnAreas; // �� ������ ���� ���� ����
    public GameObject[] doors; // ������ �����ϴ� ����
    public GameObject[] monsterPrefabs; // ���� ������
    public GameObject[] portals; // ������ ��Ż ������Ʈ

    private int currentSpawnAreaIndex = 0; // ���� ���� �ε���
    private int monsterCount; // ���� ������ ���� ��

    private void Start()
    {
        // ù ������ ���� ���� ��� (���ʹ� �÷��̾ ���� �� ����)
        OpenCurrentArea();
    }

    // Ư�� ������ ���� ����
    private void SpawnMonstersInArea(int areaIndex)
    {
        if (areaIndex >= spawnAreas.Length) return;

        Transform spawnArea = spawnAreas[areaIndex].transform;
        foreach (Transform spawnPoint in spawnArea)
        {
            int randomIndex = Random.Range(0, monsterPrefabs.Length);
            Instantiate(monsterPrefabs[randomIndex], spawnPoint.position, Quaternion.identity);
        }

        monsterCount = spawnArea.childCount;
        Debug.Log($"���� {areaIndex}�� {monsterCount} ������ ���Ͱ� �����Ǿ����ϴ�.");
    }

    // �÷��̾ Ư�� ������ ������ �� ȣ��Ǵ� �Լ�
    public void OnPlayerEnterArea(int areaIndex)
    {
        if (areaIndex == currentSpawnAreaIndex)
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

        if (monsterCount <= 0)
        {
            Debug.Log("���� ������ ��� ���Ͱ� óġ�Ǿ����ϴ�.");
            OpenNextArea();
            ActivatePortal(currentSpawnAreaIndex); // ���� ������ �´� ��Ż Ȱ��ȭ
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

    // ��Ż Ȱ��ȭ
    private void ActivatePortal(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < portals.Length)
        {
            portals[areaIndex].SetActive(true); // �ش� ������ ��Ż Ȱ��ȭ
            Debug.Log($"���� {areaIndex}�� ��Ż�� Ȱ��ȭ�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogWarning("��ȿ���� ���� areaIndex�Դϴ�.");
        }
    }

    // �� ���¸� �����ϴ� �Լ�
    public void ToggleDoor(int index, bool isActive)
    {
        if (index >= 0 && index < doors.Length)
        {
            doors[index].SetActive(isActive);
            Debug.Log($"���� {index}�� ���� {(isActive ? "Ȱ��ȭ" : "��Ȱ��ȭ")}�Ǿ����ϴ�.");
        }
    }
}
