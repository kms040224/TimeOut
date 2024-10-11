using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject portalPrefab; // ��Ż ������
    public Transform portalSpawnPoint; // ��Ż�� ������ ��ġ
    private GameObject[] monsters;

    void Start()
    {
        // ���� �±׸� ���� ��� ������Ʈ�� ã�� �迭�� ����
        monsters = GameObject.FindGameObjectsWithTag("Monster");
    }

    void Update()
    {
        CheckMonsters();
    }

    // ��� ���Ͱ� �׾����� Ȯ���ϴ� �Լ�
    void CheckMonsters()
    {
        foreach (GameObject monster in monsters)
        {
            if (monster != null) return; // �ϳ��� ��������� �Լ� ����
        }

        // ��� ���Ͱ� �׾����� ��Ż ����
        SpawnPortal();
    }

    // ��Ż�� �����ϴ� �Լ�
    void SpawnPortal()
    {
        // ��Ż�� �̹� �����Ǿ����� Ȯ��
        if (portalPrefab != null && !portalPrefab.activeSelf)
        {
            portalPrefab.SetActive(true); // ��Ż Ȱ��ȭ
            portalPrefab.transform.position = portalSpawnPoint.position; // ��Ż ��ġ ����
        }
    }
}