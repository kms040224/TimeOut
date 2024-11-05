using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance; // �̱��� ���� ����
    public GameObject magicAttackPrefab;
    public int poolSize = 10;

    private Queue<GameObject> magicAttackPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject magicAttack = Instantiate(magicAttackPrefab);
            magicAttack.SetActive(false);
            magicAttackPool.Enqueue(magicAttack);
        }
    }

    public GameObject GetMagicAttack()
    {
        if (magicAttackPool.Count > 0)
        {
            GameObject magicAttack = magicAttackPool.Dequeue();
            magicAttack.SetActive(true);
            return magicAttack;
        }
        else
        {
            // Ǯ�� �����ִ� magicAttack�� ���� ��� ���� �����ؼ� ��ȯ
            GameObject magicAttack = Instantiate(magicAttackPrefab);
            return magicAttack;
        }
    }

    public void ReturnMagicAttack(GameObject magicAttack)
    {
        magicAttack.SetActive(false);
        magicAttackPool.Enqueue(magicAttack);
    }
}