using UnityEngine;
using UnityEngine.UI;

public class BuController : MonoBehaviour
{
    public GameObject recordButton; 
    public recordaudio recordAudioScript;  

    public GameObject stopButton;

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
           
        if (recordButton != null) recordButton.SetActive(true);
        if (stopButton != null) stopButton.SetActive(false);
    }

    public void ShowRecordButton()
    {
       
        if (recordButton != null)
        {
            recordButton.SetActive(true); 
        }
    }

    
    public void OnRecordButtonClick()
    {
        recordAudioScript.StartRecordingAndSendToAPI();
        
        if (stopButton != null) stopButton.SetActive(true);
    }

    public void OnStopButtonClick()
    {
        recordAudioScript.StopRecording();
        
        if (stopButton != null) stopButton.SetActive(false);
    }
}


//원래 내 코드
// using UnityEngine;
// using UnityEngine.UI;

// public class BuController : MonoBehaviour
// {
//     public GameObject recordButton; // ���� ��ư ������Ʈ
//     public recordaudio recordAudioScript; // ���� ��ũ��Ʈ ����

//     void Start()
//     {
//         if (recordButton == null)
//         {
//             Debug.LogError("recordButton is not assigned! Please assign it in the Inspector.");
//         }
//         else
//         {
//             Debug.Log("recordButton is properly assigned.");
//             recordButton.SetActive(false);
//         }

//         if (recordAudioScript == null)
//         {
//             Debug.LogError("recordAudioScript is not assigned! Please assign it in the Inspector.");
//         }
//         else
//         {
//             Debug.Log("recordAudioScript is properly assigned.");
//         }
//         // ���� ���� �� ���� ��ư ����
//         recordButton.SetActive(false);
//     }

//     public void ShowRecordButton()
//     {
//         Debug.Log("ShowRecordButton() called.");
//         if (recordButton != null)
//         {
//             recordButton.SetActive(true);
//             Debug.Log("Record button activated.");
//         }
//         else
//         {
//             Debug.LogError("recordButton is null in ShowRecordButton!");
//         }
//     }

//     public void OnRecordButtonClick()
//     {
//         // ��ư Ŭ�� �� ���� ����
//         recordAudioScript.StartRecordingAndSendToAPI();
//     }
// }
