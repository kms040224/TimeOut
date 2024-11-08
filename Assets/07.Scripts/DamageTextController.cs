using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageTextController : MonoBehaviour
{
    private static DamageTextController _instance = null;

    public static DamageTextController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DamageTextController>();

                if (_instance == null)
                {
                    Debug.LogError("There's no active DamageTextController Object");
                }
            }

            return _instance;
        }
    }

    public Canvas canvas;

    public void CreateDamageText(Vector3 hitPoint, int hitDamage)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(hitPoint);

        GameObject damageText = ObjectPool.Instance.GetDamageText(); // ObjectPool���� DamageText ��������
        damageText.transform.SetParent(canvas.transform, false);
        damageText.transform.position = screenPosition;

        Text textComponent = damageText.GetComponent<Text>();
        if (textComponent != null)
        {
            textComponent.text = hitDamage.ToString();
        }
    }
}