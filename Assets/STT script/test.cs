using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;


public class test : MonoBehaviour
{
    DatabaseReference m_Reference;

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

                for (int i = 4; i < 5; i++)
                {
                    WriteUserData(i, "aaaa");
           
                }

                ReadUserData();
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ReadUserData()
    {
        /*
         키를 기준으로 정렬하여 최신 노드부터 가져오게 함.
         */
        FirebaseDatabase.DefaultInstance.GetReference("users")
            .OrderByKey()
            //.LimitToLast(5) // 마지막 5개 데이터만 가져오기
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    // 오류 처리
                    Debug.LogError("Error reading data: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<string> userList = new List<string>();

                    // 데이터 출력
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        string Id = childSnapshot.Key;
                        string score = childSnapshot.Child("score").Value.ToString();
                        userList.Add($"ID: {Id}, score: {score}, EMGsenser: , date: ");
                    }

                    userList.Reverse(); // 역순으로 가져온 데이터를 Reverse

                    foreach (var data in userList)
                    {
                        Debug.Log(data);
                    }
                }
            });
    }

    void WriteUserData(int id, string score)
    {

        if (m_Reference == null)
        {
            Debug.LogError("Database reference is not initialized.");
            return;
        }

        string idKey = id.ToString();

        m_Reference.Child("users").Child(idKey).Child("score").SetValueAsync(score).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to write data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Data written successfully : " + score);
            }
        });

    }
}