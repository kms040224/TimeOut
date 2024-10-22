using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    public Image fadeImage; // ���̵� �̹���
    public float fadeSpeed = 1.0f; // ���̵� �ӵ�

    private void Awake()
    {
        if (fadeImage != null)
        {
            // ó���� ���̵� �̹����� ������ �ʵ��� �����ϰ� ����
            fadeImage.color = new Color(0, 0, 0, 0);
        }
    }

    // ���̵� �� (ȭ���� ��Ӱ�)
    public IEnumerator FadeIn(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration); // ���� -> ������
            fadeImage.color = color;
            yield return null;
        }
        color.a = 1f;
        fadeImage.color = color;
    }

    // ���̵� �ƿ� (ȭ���� ���)
    public IEnumerator FadeOut(float duration)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration); // ������ -> ����
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;
    }
}