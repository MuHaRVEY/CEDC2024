using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI; // UI 관련 네임스페이스 추가
using TMPro; // TextMeshPro 네임스페이스 추가
using CookingStar;



public class ScoreManager : MonoBehaviour
{
    private DatabaseReference m_Reference;

    public CustomerController CustomerController;

    private double totalScore;  // 발음 총 점수
    private string averageScore;  // 발음 평균 점수
    private double EGMScore;
    private int productIngredients; // 해당 햄버거 재료 개수

    public GameObject Text_Notice;      // 12.23 추가: 

    // Start is called before the first frame update
    void Start()
    {
        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.Create(new AppOptions()
                {
                    DatabaseUrl = new System.Uri("https://hambugi-65e1d-default-rtdb.firebaseio.com/") // Database URL 설정
                });
                Debug.Log("Firebase Initialized"); // 연결 성공

                // Database 참조 초기화
                m_Reference = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
            }
        });

        // 총 점수 초기화
        totalScore = 0;
        EGMScore = 0;


        // UI 업데이트
        UpdateScoreUI();
    }

    public void setNumberofIngredients(int num)
    {
        // 해당 햄버거 재료 개수 가져오기
        productIngredients = num;
    }

    public string getAverageScore()
    {
        Debug.Log("넘: " + productIngredients);
        averageScore = (totalScore/ productIngredients).ToString("F2"); // 소수점 2자리까지 포매팅


        return averageScore;
    }

    // 점수 UI 업데이트
    private void UpdateScoreUI()
    {
        if (Text_Notice != null)
        {
            TMP_Text scoreText = Text_Notice.GetComponent<TMP_Text>();
            if (scoreText != null)
            {
                scoreText.text = "hambuger score :  " + totalScore.ToString("F4");
            }
            else
            {
                Debug.LogError("No Text component found on the NoticeScore object.");
            }
        }
        else
        {
            Debug.LogError("NoticeScore is not assigned.");
        }
    }

    public double getPronunciationScore()
    {
        return totalScore;
    }



    public void AddPronunciationScore(double responseScore)
    {

        totalScore += responseScore;
        UpdateScoreUI(); // UI 업데이트
    }

    public void AddEGMScore(double score)
    {

        EGMScore = score;
    }


    // DB에 총 결과 저장
    public void SaveAtDB()
    {

        if (m_Reference == null)
        {
            Debug.LogError("Database reference is not initialized.");
            return;
        }


        // 부모 노드 "results"의 자식 노드 개수 확인 후 새 노드 생성
        m_Reference.Child("results").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to retrieve data: " + task.Exception);
                return;
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // 현재 노드의 개수를 기반으로 새로운 부모 노드 번호 설정
                int nextNodeNumber = (int)snapshot.ChildrenCount + 1;

                // 새 데이터를 저장
                string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                m_Reference.Child("results").Child(nextNodeNumber.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(new GameResult
                {
                    pronunciationScore = averageScore,
                    emgScore = EGMScore,
                    timestamp = timestamp
                })).ContinueWithOnMainThread(saveTask => {
                    if (saveTask.IsCompleted)
                    {
                        Debug.Log($"Game result saved under node {nextNodeNumber}");
                    }
                    else if (saveTask.IsFaulted)
                    {
                        Debug.LogError("Failed to save game result: " + saveTask.Exception);
                    }
                });
            }
        });
    }
            
    // 발음 게임 결과 데이터 클래스
    [System.Serializable]
    public class GameResult
    {
        public string pronunciationScore; // 발음 점수
        public double emgScore;           // 근전도 점수
        public string timestamp;       // 저장 시간
    }
}
