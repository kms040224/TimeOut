using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance; // �̱��� ���� ����

    public GameObject magicAttackPrefab;
    public GameObject damageTextPrefab; // DamageText ������
    public int poolSize = 10;

    private Queue<GameObject> magicAttackPool = new Queue<GameObject>();
    private Queue<GameObject> damageTextPool = new Queue<GameObject>();

    void Awake()
    {
        Instance = this;
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            // MagicAttack ������Ʈ Ǯ �ʱ�ȭ
            GameObject magicAttack = Instantiate(magicAttackPrefab);
            magicAttack.SetActive(false);
            magicAttackPool.Enqueue(magicAttack);

            // DamageText ������Ʈ Ǯ �ʱ�ȭ
            GameObject damageText = Instantiate(damageTextPrefab);
            damageText.SetActive(false);
            damageTextPool.Enqueue(damageText);
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
            return Instantiate(magicAttackPrefab);
        }
    }

    public void ReturnMagicAttack(GameObject magicAttack)
    {
        magicAttack.SetActive(false);
        magicAttackPool.Enqueue(magicAttack);
    }

    public GameObject GetDamageText()
    {
        if (damageTextPool.Count > 0)
        {
            GameObject damageText = damageTextPool.Dequeue();
            damageText.SetActive(true);
            return damageText;
        }
        else
        {
            return Instantiate(damageTextPrefab);
        }
    }

    public void ReturnDamageText(GameObject damageText)
    {
        damageText.SetActive(false);
        damageTextPool.Enqueue(damageText);
    }
}