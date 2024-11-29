using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownUI : MonoBehaviour
{
    public Image cooldownOverlay;
    public float cooldownTime;

    private float cooldownTimer; 
    private bool isOnCooldown; 

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            
            float fillAmount = cooldownTimer / cooldownTime;
            cooldownOverlay.fillAmount = fillAmount;

           
            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
                cooldownOverlay.fillAmount = 0; 
            }
        }
    }

    
    public void StartCooldown()
    {
        cooldownTimer = cooldownTime;
        isOnCooldown = true;
        cooldownOverlay.fillAmount = 1;
    }
}