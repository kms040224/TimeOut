using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance; // �̱��� �ν��Ͻ�
    public float health = 100f; // �÷��̾��� ü�� (float�� ����)
    public float maxHealth = 100f; // �ִ� ü�� (float�� ����)
    public Text healthText; // ���� ü���� ǥ���� Text UI ������Ʈ

    private float healthDecreaseAmount = 0.01f; // ������ HP ��
    private float decreaseInterval = 0.01f; // ������ �ð� ����

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

    void Start()
    {
        UpdateHealthText(); // �ʱ� ü�� �ؽ�Ʈ ������Ʈ
        StartCoroutine(DecreaseHealthOverTime()); // HP ���� �ڷ�ƾ ����
    }

    private IEnumerator DecreaseHealthOverTime()
    {
        while (health > 0)
        {
            health -= healthDecreaseAmount; // HP ����
            if (health < 0f)
            {
                health = 0f; // ü�� ���Ѽ�
            }
            UpdateHealthText(); // ü�� �ؽ�Ʈ ������Ʈ
            yield return new WaitForSeconds(decreaseInterval); // ���
        }

        // HP�� 0�� �Ǿ��� ���� ������ ���⿡ �߰��� �� �ֽ��ϴ�.
        Debug.Log("Player has died!");
    }

    public void TakeDamage(float damage) // damage�� float�� ����
    {
        health -= damage;
        if (health < 0f)
        {
            health = 0f; // ü�� ���Ѽ�
        }
        UpdateHealthText(); // ü�� �ؽ�Ʈ ������Ʈ
    }

    public void Heal(float amount) // amount�� float�� ����
    {
        health += amount;
        if (health > maxHealth) // �ִ� ü�� ����
        {
            health = maxHealth;
        }
        UpdateHealthText(); // ü�� �ؽ�Ʈ ������Ʈ
    }

    public float CurrentHealth
    {
        get { return health; } // ���� ü�� ��������
    }

    // ���� ü���� �ؽ�Ʈ�� ������Ʈ�ϴ� �Լ�
    private void UpdateHealthText()
    {
        if (healthText != null) // healthText�� null�� �ƴ� ��쿡�� ������Ʈ
        {
            healthText.text = health.ToString("F2"); // �Ҽ��� �Ʒ� �� �ڸ����� ǥ��
        }
    }
}
