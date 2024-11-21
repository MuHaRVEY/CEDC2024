using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class record : MonoBehaviour
{
    private const string ApiKey = "AIzaSyBZZzymFa8u1VAAbUbOgqvVTwIYlckhc34"; // Google Cloud API 키
    private const string Url = "https://speech.googleapis.com/v1/speech:recognize?key=" + ApiKey;

    private AudioClip audioClip; // 녹음된 오디오 데이터
    private string Device;

    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            Debug.Log("Available Microphone: " + device);
            Device = device;

        }
    }


    // 버튼 클릭 시 호출되는 함수 (OnClick 연결)
    public void StartRecordingAndSendToAPI()
    {
        StartCoroutine(RecordAndSendToGoogle());
    }

    private IEnumerator RecordAndSendToGoogle()
    {
        Debug.Log("Recording started...");
        audioClip = Microphone.Start(Device, false, 10, 16000); // 최대 5초 녹음, 16kHz 샘플링

        // 녹음이 완료될 때까지 대기
        while (Microphone.IsRecording(Device))
        {
            yield return null; // 한 프레임 대기
        }

        Debug.Log(audioClip);

        Debug.Log("Recording stopped.");
        Microphone.End(Device);

        // 녹음된 데이터 디버깅
        if (audioClip == null)
        {
            Debug.LogError("AudioClip is null! Recording failed.");
            yield break;
        }

        // 오디오 데이터를 가져오기
        float[] samples = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(samples, 0);

        // 오디오 데이터를 PCM 형식의 byte[]로 변환
        byte[] audioData = ConvertAudioClipToPCM(audioClip);

        Debug.Log(BitConverter.ToString(audioData));
        // Google Speech-to-Text API에 전송
        yield return SendAudioToGoogle(audioData);
    }

    // 오디오 데이터를 PCM 형식으로 변환
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
        //Debug.Log("Base64 Audio Length: " + base64Audio.Length); // 길이를 확인
        //Debug.Log("Base64 Audio (Partial): " + base64Audio.Substring(0, 100) + "..."); // 일부 출력



        // JSON 요청 데이터 생성 (수동으로 작성)
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

        // JSON 직렬화
        //string json = JsonUtility.ToJson(requestBody);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(json);

        // UnityWebRequest로 POST 요청 생성
        UnityWebRequest request = new UnityWebRequest(Url, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }
}
