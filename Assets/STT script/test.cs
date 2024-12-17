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


                WriteUserData("0", "aaaa");
                WriteUserData("1", "bbbb");
                //WriteUserData("2", "cccc");
                //WriteUserData("3", "dddd");

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
        FirebaseDatabase.DefaultInstance.GetReference("users")
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
                    // 데이터 출력
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        Debug.Log("Username:" + childSnapshot.Child("username").Value.ToString());
                    }

                }
            });
    }

    void WriteUserData(string userId, string username)
    {

        if (m_Reference == null)
        {
            Debug.LogError("Database reference is not initialized.");
            return;
        }

        m_Reference.Child("users").Child(userId).Child("username").SetValueAsync(username).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to write data: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Data written successfully : " + userId);
            }
        });

    }
}