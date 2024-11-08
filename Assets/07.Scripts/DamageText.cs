using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private Text text;
    private Color originalColor;
    public float fadeDuration = 1.0f;
    public float moveSpeed = 300f;

    private void Awake()
    {
        text = GetComponent<Text>();
        originalColor = text.color;
    }

    private void OnEnable()
    {
        StartCoroutine(FadeOutAndMoveUp());
    }

    private IEnumerator FadeOutAndMoveUp()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            yield return null;
        }

        ObjectPool.Instance.ReturnDamageText(gameObject); // ObjectPool·Î ¹ÝÈ¯
    }
}