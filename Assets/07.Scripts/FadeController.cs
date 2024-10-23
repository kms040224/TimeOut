using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image fadeImage; // 페이드 이미지
    public float fadeSpeed = 1.0f; // 페이드 속도

    private void Awake()
    {
        if (fadeImage != null)
        {
            // 처음엔 페이드 이미지가 보이지 않도록 투명하게 설정
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    // 페이드 인 (화면을 어둡게)
    public IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration); // 투명 -> 불투명
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;
    }

    // 페이드 아웃 (화면을 밝게)
    public IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration); // 불투명 -> 투명
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }
}