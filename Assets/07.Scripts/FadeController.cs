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
            // ó���� ���̵� �̹����� ������ �ʵ��� �����ϰ� �����ϰ� ��Ȱ��ȭ
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }
    }

    // ���̵� �� (ȭ���� ��Ӱ�)
    public IEnumerator FadeIn(float duration)
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true); // ���̵� �̹��� Ȱ��ȭ
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
    }

    // ���̵� �ƿ� (ȭ���� ���)
    public IEnumerator FadeOut(float duration)
    {
        if (fadeImage != null)
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
            fadeImage.gameObject.SetActive(false); // ���̵� �ƿ� �Ϸ� �� ��Ȱ��ȭ
        }
    }
}
