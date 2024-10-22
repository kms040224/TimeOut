using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Item Effects/Heal")]
public class HealEffect : ItemEffect
{
    public int healAmount; // ȸ����

    public override bool ExecuteRole()
    {
        // PlayerHealthManager �̱����� ����Ͽ� ü�� ȸ��
        if (PlayerHealthManager.Instance != null)
        {
            PlayerHealthManager.Instance.Heal(healAmount);
            return true; // ���������� ȸ�������� true ��ȯ
        }
        return false; // PlayerHealthManager�� ������ false ��ȯ
    }
}