using UnityEngine;
using UnityEngine.UI;

public class BuController : MonoBehaviour
{
    public GameObject recordButton; // 녹음 버튼 오브젝트
    public recordaudio recordAudioScript; // 녹음 스크립트 참조

    void Start()
    {
        // 게임 시작 시 녹음 버튼 숨김
        recordButton.SetActive(false);
    }

    public void ShowRecordButton()
    {
        // 녹음 버튼 활성화
        if (recordButton != null)
        {
            recordButton.SetActive(true); // 녹음 버튼 활성화
        }
    }

    public void OnRecordButtonClick()
    {
        // 버튼 클릭 시 녹음 시작
        recordAudioScript.StartRecordingAndSendToAPI();
    }
}
