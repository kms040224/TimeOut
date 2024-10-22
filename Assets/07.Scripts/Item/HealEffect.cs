using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Effect", menuName = "Item Effects/Heal")]
public class HealEffect : ItemEffect
{
    public int healAmount; // 회복량

    public override bool ExecuteRole()
    {
        // PlayerHealthManager 싱글톤을 사용하여 체력 회복
        if (PlayerHealthManager.Instance != null)
        {
            PlayerHealthManager.Instance.Heal(healAmount);
            return true; // 성공적으로 회복했으면 true 반환
        }
        return false; // PlayerHealthManager가 없으면 false 반환
    }
}