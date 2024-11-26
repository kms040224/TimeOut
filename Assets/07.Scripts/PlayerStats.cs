using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Stats/PlayerStats")]
public class PlayerStats : ScriptableObject
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 10f;

    public float flamethrowerCooldown = 12f;
    public float flamethrowerDuration = 1.5f;

    public float rollCooldown = 5f;
    public float rollDistance = 10f;
    public float rollTime = 0.5f;

    public float areaEffectCooldown = 10f;
    public float meteorCooldown = 40f;

    public float magicAttackDamage = 10f; // 기본 마법 공격력

    public float flamethrowerDamageMultiplier = 0.5f; // 화염방사기 배율
    public float areaEffectDamageMultiplier = 0.3f;  // 장판 효과 배율
    public float meteorDamageMultiplier = 1.5f;      // 메테오 배율

    public float barrierCooldown = 20f;
    public float barrierDuration = 5f;

    // 스탯 업그레이드 메서드
    public void UpgradeStat(string statName, float upgradeValue)
    {
        switch (statName)
        {
            case "movementSpeed":
                movementSpeed += upgradeValue;
                Debug.Log($"Movement Speed upgraded to {movementSpeed}");
                break;

            case "magicAttackDamage":
                magicAttackDamage += upgradeValue;
                Debug.Log($"Magic Attack Damage upgraded to {magicAttackDamage}");
                break;

            case "flamethrowerCooldown":
                flamethrowerCooldown -= upgradeValue;
                Debug.Log($"Flamethrower Cooldown upgraded to {flamethrowerCooldown}");
                break;

            default:
                Debug.LogWarning($"Unknown stat: {statName}");
                break;
        }
    }
}
