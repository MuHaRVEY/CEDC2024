using UnityEngine;
using TMPro;

public class CuteMessageController : MonoBehaviour
{
    public TextMeshProUGUI messageText; // TextMeshPro 컴포넌트
    public float showDuration = 2f;    // 텍스트 표시 시간
    public float fadeSpeed = 1f;       // 텍스트 사라지는 속도

    private float timer = 0f;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = messageText.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = messageText.gameObject.AddComponent<CanvasGroup>();
        }
        ShowMessage();
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                StartCoroutine(FadeOut());
            }
        }
    }

    public void ShowMessage()
    {
        timer = showDuration;
        messageText.text = "Give it your all!";
        canvasGroup.alpha = 1f; // 텍스트를 완전히 보이게 설정
    }

    private System.Collections.IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
