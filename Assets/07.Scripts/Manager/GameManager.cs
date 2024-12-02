using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] spawnAreas; // �� ������ ���� ����
    public GameObject[] doors; // ������ �����ϴ� ��
    public GameObject[] portals; // ��Ż ������Ʈ
    private static GameManager instance;
    public static GameManager Instance
    {

        get 
        {
            
            return instance; 
        }
    }



    private int currentSpawnAreaIndex = 0; // ���� ���� �ε���
    private int currentWave = 0; // ���� ���̺� (0: ù ��° ���̺�, 1: �� ��° ���̺�)
    private int monsterCount = 0; // ���� ������ ���� ���� ��
    public PlayerStats playerStats;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); // GameManager�� �ٸ� ������ �Ѿ�� �����ǵ��� ����
    }

    public PlayerStats GetPlayerStats()
    {
        return playerStats;
    }
    private void Start()
    {

        OpenCurrentArea(); // ù ������ ���� ���ϴ�.
    }

    // Ư�� ���̺꿡�� ���� ����
    private void SpawnMonstersInWave(int areaIndex, int waveIndex)
    {
        if (areaIndex >= spawnAreas.Length) return;

        Transform spawnArea = spawnAreas[areaIndex].transform;
        Transform wave = spawnArea.Find($"Wave{waveIndex + 1}");

        if (wave == null)
        {
            Debug.Log($"Wave{waveIndex + 1}�� ���� {areaIndex}�� �����ϴ�.");
            OnAllWavesCleared(); // ���� ���̺갡 ������ ��� ���̺� �Ϸ� ó��
            return;
        }

        foreach (Transform spawnPoint in wave)
        {
            // SpawnPoint ��ũ��Ʈ�� ������
            SpawnPoint spawnPointScript = spawnPoint.GetComponent<SpawnPoint>();

            if (spawnPointScript != null && spawnPointScript.assignedMonster != null)
            {
                Instantiate(spawnPointScript.assignedMonster, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"���� ����Ʈ {spawnPoint.name}�� �Ҵ�� ���Ͱ� �����ϴ�.");
            }
        }

        monsterCount = wave.childCount;
        Debug.Log($"���� {areaIndex}�� Wave{waveIndex + 1}�� {monsterCount} ������ ���Ͱ� �����Ǿ����ϴ�.");
    }

    // �÷��̾ Ư�� ������ ������ �� ȣ��Ǵ� �Լ�
    public void OnPlayerEnterArea(int areaIndex)
    {
        if (areaIndex == currentSpawnAreaIndex)
        {
            Debug.Log($"�÷��̾ ���� {areaIndex}�� ���Խ��ϴ�.");
            SpawnMonstersInWave(areaIndex, currentWave); // ù ��° ���̺� ����
        }
    }

    // ���Ͱ� ���� �� ȣ��Ǵ� �Լ�
    public void OnMonsterKilled()
    {
        monsterCount--;
        Debug.Log("���� ���� ��: " + monsterCount);

        if (monsterCount <= 0)
        {
            Debug.Log($"Wave{currentWave + 1}�� ��� ���Ͱ� óġ�Ǿ����ϴ�.");
            currentWave++;

            SpawnMonstersInWave(currentSpawnAreaIndex, currentWave); // ���� ���̺� ����
        }
    }

    // ��� ���̺갡 ������ �� ȣ��
    private void OnAllWavesCleared()
    {
        Debug.Log("���� ������ ��� ���̺갡 �Ϸ�Ǿ����ϴ�.");

        OpenNextArea();
        ActivatePortal(currentSpawnAreaIndex); // ��Ż Ȱ��ȭ

        // StatUpgradeUI �ν��Ͻ� Ȯ�� �� Ȱ��ȭ
        if (StatUpgradeUI.Instance == null)
        {
            GameObject uiObject = GameObject.Find("StatUpgradeUI"); // ������Ʈ �̸����� ã��
            if (uiObject != null)
            {
                uiObject.SetActive(true); // Ȱ��ȭ
                StatUpgradeUI.Instance = uiObject.GetComponent<StatUpgradeUI>(); // ���� ����
            }
        }

        if (StatUpgradeUI.Instance != null)
        {
            StatUpgradeUI.Instance.ShowPanel();
        }
        else
        {
            Debug.LogError("StatUpgradeUI.Instance�� null�Դϴ�. StatUpgradeUI�� ���� �߰��Ǿ����� Ȯ���ϼ���.");
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
        currentWave = 0; // ���̺긦 �ʱ�ȭ
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
