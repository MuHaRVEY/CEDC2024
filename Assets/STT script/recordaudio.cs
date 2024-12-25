using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using CookingStar;


public class recordaudio : MonoBehaviour
{
    private const string ApiKey = "AIzaSyDOb7GFYcZJs4sySTVgEzBUPlMcQvo2Duk"; // Google Cloud API 키
    private const string Url = "https://speech.googleapis.com/v1/speech:recognize?key=" + ApiKey;
    public IngredientsController ingredientsController;
    public ScoreManager ScoreManager;
    private AudioClip audioClip; // 오디오 클립 객체               
    private string Device;  // 사용 중인 마이크 장치 이름
    private bool isRecording = false;

    void Start()
    {
        // 사용 가능한 마이크 장치 출력
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Available Microphone: " + device);
            Device = device; // 첫 번째 마이크 장치를 기본으로 설정

        }
    }


    // 녹음 시작 함수 (버튼 클릭 시 호출)
    public void StartRecordingAndSendToAPI()
    {
        // StartCoroutine(RecordAndSendToGoogle());
        if (isRecording)
        {
            Debug.LogWarning("Already recording!");
            return;
        }

        Debug.Log("Recording started...");
        audioClip = Microphone.Start(Device, false, 10, 16000); // 최대 10초 녹음 가능
        isRecording = true;
    }

    // 녹음 중지 함수
    public void StopRecording()
    {
        if (!isRecording)
        {
            Debug.LogWarning("No active recording to stop!");
            return;
        }

        Debug.Log("Recording stopped.");
        Microphone.End(Device); // 마이크 녹음 중지
        isRecording = false;

        // 녹음 완료 후 오디오 데이터 처리
        if (audioClip != null)
        {
            StartCoroutine(RecordAndSendToGoogle());
        }
        else
        {
            Debug.LogError("AudioClip is null! Recording failed.");
        }
    }

    // Google API로 오디오 데이터를 전송
    public IEnumerator RecordAndSendToGoogle()
    {
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        // 오디오 데이터를 PCM(byte[]) 형식으로 변환
        byte[] audioData = ConvertAudioClipToPCM(audioClip);

        Debug.Log(BitConverter.ToString(audioData));

        // Google Speech-to-Text API 호출
        yield return SendAudioToGoogle(audioData);
    }

    // AudioClip 데이터를 PCM 형식으로 변환
    private byte[] ConvertAudioClipToPCM(AudioClip clip)
    {
        float[] samples = new float[clip.samples];
        clip.GetData(samples, 0);

        byte[] pcmData = new byte[samples.Length * 2];
        int index = 0;

        foreach (float sample in samples)
        {
            short pcmSample = (short)(sample * 32767);
            pcmData[index++] = (byte)(pcmSample & 0xff);
            pcmData[index++] = (byte)((pcmSample >> 8) & 0xff);
        }

        return pcmData;
    }

    // Google Speech-to-Text API로 오디오 데이터 전송
    private IEnumerator SendAudioToGoogle(byte[] audioData)
    {
        Debug.Log("Sending audio to Google API...");

        // 오디오 데이터를 Base64로 인코딩
        string base64Audio = System.Convert.ToBase64String(audioData);


        // JSON 요청 데이터 생성
        string json = $@"
        {{
        ""config"": {{
            ""encoding"": ""LINEAR16"",
            ""sampleRateHertz"": 16000,
            ""languageCode"": ""ko-KR""
        }},
        ""audio"": {{
            ""content"": ""{base64Audio}""
        }}
        }}";

        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        // UnityWebRequest를 사용하여 POST 요청    
        UnityWebRequest request = new UnityWebRequest(Url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 전송    
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            //Debug.Log("Response: " + request.downloadHandler.text);
            try
            {
                ProcessGoogleResponse(request.downloadHandler.text);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in ProcessGoogleResponse: {ex.Message}");
            }
        }
        else
        {
            Debug.Log("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    

    // Google API 응답 처리
    private void ProcessGoogleResponse(string jsonResponse)
    {
        Debug.Log("Processing Google Response...");

        // JSON 응답 파싱
        try
        {
            var response = JsonUtility.FromJson<GoogleSpeechResponse>(jsonResponse);


            if (response != null && response.results != null && response.results.Length > 0)
            {
                string recognizedWord = response.results[0].alternatives[0].transcript.Trim();
                double confidenceScore = response.results[0].alternatives[0].confidence;
                //Debug.Log($"Recognized Word: {recognizedWord}");
                //Debug.Log($"confidenceScore: {confidenceScore}");

                // 각 발음 점수 저장
                ScoreManager.AddPronunciationScore(confidenceScore);

                

                if (ingredientsController != null)
                {
                    Debug.Log("Calling IngredientsController.SelectIngredientByWord...");
                    ingredientsController.SelectIngredientByWord(recognizedWord);
                }
                else
                {
                    Debug.LogError("IngredientsController is not assigned in recordaudio!");
                }
            }
            else
            {
                Debug.LogError("No transcription found in Google API response.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing Google Response: {ex.Message}");
        }

    }

    

    [System.Serializable]
    public class GoogleSpeechResponse
    {
        public Result[] results;
    }

    [System.Serializable]
    public class Result
    {
        public Alternative[] alternatives;
    }

    [System.Serializable]
    public class Alternative
    {
        public string transcript;
        public double confidence; // confidence 점수
    }

}


// using System.Collections;
// using System.Text;
// using UnityEngine;
// using UnityEngine.Networking;
// using System;
// using CookingStar;

// public class recordaudio : MonoBehaviour
// {
//     private const string ApiKey = "AIzaSyBZZzymFa8u1VAAbUbOgqvVTwIYlckhc34"; // Google Cloud API Ű
//     private const string Url = "https://speech.googleapis.com/v1/speech:recognize?key=" + ApiKey;
//     public IngredientsController ingredientsController;
//     private AudioClip audioClip; // ������ ����� ������
//     private string Device;

//     void Start()
//     {
//         foreach (var device in Microphone.devices)
//         {
//             Debug.Log("Available Microphone: " + device);
//             Device = device;

//         }
//     }


//     // ��ư Ŭ�� �� ȣ��Ǵ� �Լ� (OnClick ����)
//     public void StartRecordingAndSendToAPI()
//     {
//         StartCoroutine(RecordAndSendToGoogle());
//     }

//     private IEnumerator RecordAndSendToGoogle()
//     {
//         Debug.Log("Recording started...");
//         audioClip = Microphone.Start(Device, false, 10, 16000); // �ִ� 5�� ����, 16kHz ���ø�

//         // ������ �Ϸ�� ������ ���
//         while (Microphone.IsRecording(Device))
//         {
//             yield return null; // �� ������ ���
//         }

//         Debug.Log(audioClip);

//         Debug.Log("Recording stopped.");
//         Microphone.End(Device);

//         // ������ ������ �����
//         if (audioClip == null)
//         {
//             Debug.LogError("AudioClip is null! Recording failed.");
//             yield break;
//         }

//         // ����� �����͸� ��������
//         float[] samples = new float[audioClip.samples * audioClip.channels];
//         audioClip.GetData(samples, 0);

//         // ����� �����͸� PCM ������ byte[]�� ��ȯ
//         byte[] audioData = ConvertAudioClipToPCM(audioClip);

//         Debug.Log(BitConverter.ToString(audioData));
//         // Google Speech-to-Text API�� ����
//         yield return SendAudioToGoogle(audioData);
//     }

//     // ����� �����͸� PCM �������� ��ȯ
//     private byte[] ConvertAudioClipToPCM(AudioClip clip)
//     {
//         float[] samples = new float[clip.samples];
//         clip.GetData(samples, 0);

//         byte[] pcmData = new byte[samples.Length * 2];
//         int index = 0;

//         foreach (float sample in samples)
//         {
//             short pcmSample = (short)(sample * 32767);
//             pcmData[index++] = (byte)(pcmSample & 0xff);
//             pcmData[index++] = (byte)((pcmSample >> 8) & 0xff);
//         }

//         return pcmData;
//     }

//     // Google Speech-to-Text API�� ����� ������ ����
//     private IEnumerator SendAudioToGoogle(byte[] audioData)
//     {
//         Debug.Log("Sending audio to Google API...");

//         // ����� �����͸� Base64�� ���ڵ�
//         string base64Audio = System.Convert.ToBase64String(audioData);
//         //Debug.Log("Base64 Audio Length: " + base64Audio.Length); // ���̸� Ȯ��
//         //Debug.Log("Base64 Audio (Partial): " + base64Audio.Substring(0, 100) + "..."); // �Ϻ� ���



//         // JSON ��û ������ ���� (�������� �ۼ�)
//         string json = $@"
//     {{
//       ""config"": {{
//         ""encoding"": ""LINEAR16"",
//         ""sampleRateHertz"": 16000,
//         ""languageCode"": ""ko-KR""
//       }},
//       ""audio"": {{
//         ""content"": ""{base64Audio}""
//       }}
//     }}";

//         // JSON ����ȭ
//         //string json = JsonUtility.ToJson(requestBody);
//         byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

//         // UnityWebRequest�� POST ��û ����
//         UnityWebRequest request = new UnityWebRequest(Url, "POST");
//         request.uploadHandler = new UploadHandlerRaw(jsonBytes);
//         request.downloadHandler = new DownloadHandlerBuffer();
//         request.SetRequestHeader("Content-Type", "application/json");

//         // ��û ������
//         yield return request.SendWebRequest();

//         // ��� ó��
//         if (request.result == UnityWebRequest.Result.Success)
//         {
//             Debug.Log("Response: " + request.downloadHandler.text);
//             try
//             {
//                 // ����� �α� �߰�
//                 Debug.Log("Calling ProcessGoogleResponse...");
//                 ProcessGoogleResponse(request.downloadHandler.text);
//             }
//             catch (Exception ex)
//             {
//                 Debug.LogError($"Error in ProcessGoogleResponse: {ex.Message}");
//             }
//         }
//         else
//         {
//             Debug.Log("Error: " + request.error);
//             Debug.LogError("Response: " + request.downloadHandler.text);
//         }
//     }

//     private void ProcessGoogleResponse(string jsonResponse)
//     {
//         Debug.Log("Processing Google Response...");

//         // JSON ���信�� ��ȯ�� �ؽ�Ʈ�� ����
//         try
//         {
//             // Simple JSON ��� (�Ǵ� JSON ���� ���� �м�)
//             var response = JsonUtility.FromJson<GoogleSpeechResponse>(jsonResponse);

//             if (response != null && response.results != null && response.results.Length > 0)
//             {
//                 string recognizedWord = response.results[0].alternatives[0].transcript.Trim();
//                 Debug.Log($"Recognized Word: {recognizedWord}");

//                 // IngredientsController�� �ܾ� ����
//                 if (ingredientsController != null)
//                 {
//                     Debug.Log("Calling IngredientsController.SelectIngredientByWord...");
//                     ingredientsController.SelectIngredientByWord(recognizedWord);
//                 }
//                 else
//                 {
//                     Debug.LogError("IngredientsController is not assigned in recordaudio!");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("No transcription found in Google API response.");
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"Error processing Google Response: {ex.Message}");
//             Debug.LogError($"Raw Response: {jsonResponse}");
//         }

//     }

//     [System.Serializable]
//     public class GoogleSpeechResponse
//     {
//         public Result[] results;
//     }

//     [System.Serializable]
//     public class Result
//     {
//         public Alternative[] alternatives;
//     }

//     [System.Serializable]
//     public class Alternative
//     {
//         public string transcript;
//     }

// }
