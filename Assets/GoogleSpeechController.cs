// using UnityEngine;
// using Google.Cloud.Speech.V1;
// using System;
// using System.IO;

// public class GoogleSpeechController : MonoBehaviour
// {
//     private SpeechClient speechClient;
//     private MicrophoneInput microphoneInput;

//     void Start()
//     {
//         // JSON 파일 경로 설정 (StreamingAssets 폴더에 JSON 파일이 있다고 가정)
//         string jsonPath = Path.Combine(Application.streamingAssetsPath, "cedc2024.json");
//         Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", jsonPath);

//         // SpeechClient 초기화
//         speechClient = SpeechClient.Create();
//         Debug.Log("Google Speech Client Initialized");
//         // 같은 GameObject에 있는 MicrophoneInput 컴포넌트를 찾음
//         microphoneInput = GetComponent<MicrophoneInput>();
        
//         // 씬 전체에서 MicrophoneInput 컴포넌트를 찾아 참조
//         microphoneInput = FindObjectOfType<MicrophoneInput>();

//         if (microphoneInput == null)
//         {
//           Debug.LogError("MicrophoneInput component not found on this GameObject.");
//         }
//     }

//     public void RecognizeSpeech(byte[] audioData)
//     {
//         var response = speechClient.Recognize(new RecognitionConfig()
//         {
//             Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
//             SampleRateHertz = 16000,
//             LanguageCode = "en-US"
//         },
//         RecognitionAudio.FromBytes(audioData));

//         foreach (var result in response.Results)
//         {
//             Debug.Log($"Recognized Text: {result.Alternatives[0].Transcript}");
//         }
//     }

//     public void StartRecognition()
//     {
//          Debug.Log("Recognition started!");

//         // MicrophoneInput에서 오디오 데이터를 가져옵니다.
//         if (microphoneInput != null)
//         {
//             byte[] audioData = microphoneInput.GetAudioData();

//             if (audioData != null)
//             {
//                 var response = speechClient.Recognize(new RecognitionConfig()
//                 {
//                     Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
//                     SampleRateHertz = 16000, // 샘플 레이트 설정
//                     LanguageCode = "en-US"  // 언어 설정
//                 },
//                 RecognitionAudio.FromBytes(audioData));

//                 foreach (var result in response.Results)
//                 {
//                     Debug.Log($"Recognized Text: {result.Alternatives[0].Transcript}");
//                 }
//             }
//             else
//             {
//                 Debug.LogError("Audio data is null. Microphone may not be recording.");
//             }
//         }
//         else
//         {
//             Debug.LogError("MicrophoneInput is not assigned or found.");
//         }
//     }

// }
