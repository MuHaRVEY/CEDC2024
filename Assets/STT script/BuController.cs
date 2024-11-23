using UnityEngine;
using UnityEngine.UI;

public class BuController : MonoBehaviour
{
    public GameObject recordButton; // ���� ��ư ������Ʈ
    public recordaudio recordAudioScript; // ���� ��ũ��Ʈ ����

    void Start()
    {
        if (recordButton == null)
        {
            Debug.LogError("recordButton is not assigned! Please assign it in the Inspector.");
        }
        else
        {
            Debug.Log("recordButton is properly assigned.");
            recordButton.SetActive(false);
        }

        if (recordAudioScript == null)
        {
            Debug.LogError("recordAudioScript is not assigned! Please assign it in the Inspector.");
        }
        else
        {
            Debug.Log("recordAudioScript is properly assigned.");
        }
        // ���� ���� �� ���� ��ư ����
        recordButton.SetActive(false);
    }

    public void ShowRecordButton()
    {
        // ���� ��ư Ȱ��ȭ
        if (recordButton != null)
        {
            recordButton.SetActive(true); // ���� ��ư Ȱ��ȭ
        }
    }

    public void OnRecordButtonClick()
    {
        // ��ư Ŭ�� �� ���� ����
        recordAudioScript.StartRecordingAndSendToAPI();
    }
}
