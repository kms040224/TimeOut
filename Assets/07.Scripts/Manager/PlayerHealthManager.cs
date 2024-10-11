using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance; // �̱��� �ν��Ͻ�
    public int health = 100; // �÷��̾��� ü��
    public int maxHealth = 100; // �ִ� ü��

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� �ı����� �ʵ��� ����
        }
        else
        {
            Destroy(gameObject); // �̹� �����ϸ� �ı�
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // ü�� ���Ѽ�
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) // �ִ� ü�� ����
        {
            health = maxHealth;
        }
    }

    public int CurrentHealth
    {
        get { return health; } // ���� ü�� ��������
    }
}