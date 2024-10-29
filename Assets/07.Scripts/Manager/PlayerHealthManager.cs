using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance; // 싱글톤 인스턴스
    public float health = 100f; // 플레이어의 체력 (float로 변경)
    public float maxHealth = 100f; // 최대 체력 (float로 변경)
    public Text healthText; // 현재 체력을 표시할 Text UI 컴포넌트

    private float healthDecreaseAmount = 0.01f; // 감소할 HP 양
    private float decreaseInterval = 0.01f; // 감소할 시간 간격

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

    void Start()
    {
        UpdateHealthText(); // 초기 체력 텍스트 업데이트
        StartCoroutine(DecreaseHealthOverTime()); // HP 감소 코루틴 시작
    }

    private IEnumerator DecreaseHealthOverTime()
    {
        while (health > 0)
        {
            health -= healthDecreaseAmount; // HP 감소
            if (health < 0f)
            {
                health = 0f; // 체력 하한선
            }
            UpdateHealthText(); // 체력 텍스트 업데이트
            yield return new WaitForSeconds(decreaseInterval); // 대기
        }

        // HP가 0이 되었을 때의 로직을 여기에 추가할 수 있습니다.
        Debug.Log("Player has died!");
    }

    public void TakeDamage(float damage) // damage를 float로 변경
    {
        health -= damage;
        if (health < 0f)
        {
            health = 0f; // 체력 하한선
        }
        UpdateHealthText(); // 체력 텍스트 업데이트
    }

    public void Heal(float amount) // amount를 float로 변경
    {
        health += amount;
        if (health > maxHealth) // 최대 체력 제한
        {
            health = maxHealth;
        }
        UpdateHealthText(); // 체력 텍스트 업데이트
    }

    public float CurrentHealth
    {
        get { return health; } // 현재 체력 가져오기
    }

    // 현재 체력을 텍스트로 업데이트하는 함수
    private void UpdateHealthText()
    {
        if (healthText != null) // healthText가 null이 아닐 경우에만 업데이트
        {
            healthText.text = health.ToString("F2"); // 소수점 아래 두 자리까지 표시
        }
    }
}
