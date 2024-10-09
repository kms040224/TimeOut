using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance; // 싱글톤 인스턴스
    public int health = 100; // 플레이어의 체력
    public int maxHealth = 100; // 최대 체력

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 이미 존재하면 파괴
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0; // 체력 하한선
        }
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) // 최대 체력 제한
        {
            health = maxHealth;
        }
    }

    public int CurrentHealth
    {
        get { return health; } // 현재 체력 가져오기
    }
}