// using UnityEngine;
// using System;

// public class MicrophoneInput : MonoBehaviour
// {
//     private AudioClip audioClip;
//     private const int sampleRate = 16000;

//     void Start()
//     {
//         // 마이크 녹음 시작
//         audioClip = Microphone.Start(null, true, 10, sampleRate);
//         Debug.Log("Microphone recording started.");
//     }

//     public byte[] GetAudioData()
//     {
//         if (Microphone.IsRecording(null))
//         {
//             int position = Microphone.GetPosition(null);
//             float[] samples = new float[audioClip.samples];
//             audioClip.GetData(samples, 0);

//             // float 배열을 byte 배열로 변환
//             byte[] audioBytes = new byte[samples.Length * sizeof(float)];
//             Buffer.BlockCopy(samples, 0, audioBytes, 0, audioBytes.Length);

//             return audioBytes;
//         }

//         Debug.LogError("Microphone is not recording.");
//         return null;
//     }
// }
