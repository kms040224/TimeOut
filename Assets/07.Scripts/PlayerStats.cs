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
    public float magicAttackDamage = 10f;
    public float barrierCooldown = 20f;
    public float barrierDuration = 5f;

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
                flamethrowerCooldown += upgradeValue;
                Debug.Log($"Flamethrower Cooldown upgraded to {flamethrowerCooldown}");
                break;
            default:
                Debug.LogWarning($"Unknown stat: {statName}");
                break;
        }
    }
}