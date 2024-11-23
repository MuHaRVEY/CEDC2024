using UnityEngine;
using System.Collections.Generic;
using System.IO.Ports;
using System.Collections;

public class BurgerPacking : MonoBehaviour
{
    public Transform burgerForPacking;
    public Transform pack;
    public float moveSpeed = 1f;
    private bool hasArrived = false;

    private bool isMoving = false;

    [Header("Graph Settings")]
    public LineRenderer lineRenderer;
    public LineRenderer thresholdLine;
    public int maxPoints = 100;
    public float xSpacing = 1f;
    public float yScale = 1f;
    public float maxDataValue = 1000f;
    public RectTransform GraphBackground;
    public RectTransform graphCanvas;

    private const float graphWidth = 15f;
    private const float graphHeight = 7f;

    [Header("UI Settings")]
    public GameObject successMessage;
    public GameObject failMessage;

    [Header("Sensor Settings")]
    public string portName = "COM6";
    public int baudRate = 9600;
    public float emgThreshold = 10f;
    public float packingTime = 15f;

    private SerialPort serialPort;
    private Queue<float> dataPoints = new Queue<float>();
    private List<Vector3> graphPoints = new List<Vector3>();

    private float packingTimer;
    private float elapsedTime = 0f;
    private bool isPackingActive = false;
    private bool isPackingSuccess = false;

    public GameObject gameOverPanel; // 게임 오버 창


    void Start()
{
    // Serial port initialization
    serialPort = new SerialPort(portName, baudRate);
    try
    {
        serialPort.Open();
        Debug.Log("Serial Port Opened: " + portName);
    }
    catch (System.Exception e)
    {
        Debug.LogError("Failed to open serial port: " + e.Message);
        serialPort = null; // 포트를 null로 설정
    }

    // 게임 오버 창 비활성화
    if (gameOverPanel != null)
    {
        gameOverPanel.SetActive(false);
    }

    // Graph scaling
    ScaleGraphToBackground();

    // Graph initialization
    lineRenderer.positionCount = 0;
    DrawThreshold();

    // UI initialization
    if (successMessage != null) successMessage.SetActive(false);
    if (failMessage != null) failMessage.SetActive(false);
}

void Update()
{
    if (!IsSensorConnected())
    {
        Debug.LogWarning("Sensor is not connected. Packing cannot start.");
        return; // 센서가 연결되지 않았으면 Update를 중단
    }

    if (isPackingActive)
    {
        packingTimer -= Time.deltaTime;
        elapsedTime += Time.deltaTime;

        if (packingTimer <= 0)
        {
            FinalizePacking(false); // Packing failed
        }
        else
        {
            ReadEMGData();
            UpdateGraph();
        }
    }

    // Start packing when spacebar is pressed
    if (Input.GetKeyDown(KeyCode.Space) && !isMoving && !hasArrived)
    {
        StartPacking();
        Debug.Log("Packing started.");
    }

    if (isMoving)
    {
        burgerForPacking.position = Vector3.Lerp(
            burgerForPacking.position,
            pack.position,
            Time.deltaTime * moveSpeed
        );

        // 목표 위치에 도달하면 이동 멈추고 위치를 정확히 설정
        if (Vector3.Distance(burgerForPacking.position, pack.position) < 0.1f)
        {
            isMoving = false;
            hasArrived = true;
            burgerForPacking.position = pack.position; // 정확히 포장지 위치로 설정
            burgerForPacking.localScale = Vector3.zero; // 크기를 0으로 설정 (사라지듯 연출)
            Debug.Log("Burger reached the pack!");

            // 게임 오버 창 활성화
            ShowGameOverPanel();
        }
    }
}

/// <summary>
/// 센서 연결 여부 확인
/// </summary>
    private bool IsSensorConnected()
    {
        // serialPort가 null이거나 열려 있지 않으면 센서가 연결되지 않음
        if (serialPort == null || !serialPort.IsOpen)
        {
            return false;
        }
        return true;
    }


    private void StartPacking()
    {
        Debug.Log("Packing started!");
        isPackingActive = true;
        packingTimer = packingTime;
        elapsedTime = 0f;
        isPackingSuccess = false;

        if (successMessage != null) successMessage.SetActive(false);
        if (failMessage != null) failMessage.SetActive(false);
    }

    private void ReadEMGData()
    {
        float emgValue = 0;

        if (serialPort.IsOpen)
        {
            try
            {
                string data = serialPort.ReadLine();
                if (float.TryParse(data, out emgValue))
                {
                    Debug.Log($"Received EMG Data: {emgValue}");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error reading EMG data: " + e.Message);
            }
        }

        AddDataPoint(emgValue);

        if (emgValue >= emgThreshold)
        {
            FinalizePacking(true);
        }
    }

    private void AddDataPoint(float emgValue)
    {
        if (dataPoints.Count >= maxPoints)
        {
            dataPoints.Dequeue();
        }
        dataPoints.Enqueue(emgValue);
    }

    // private void UpdateGraph()
    // {
    //     if (lineRenderer == null) return;

    //     Vector3[] graphPositions = new Vector3[maxPoints];
    //     float xSpacing = graphWidth / maxPoints; // X-axis spacing based on graph width

    //     int index = 0;
    //     foreach (float value in dataPoints)
    //     {
    //         float x = index * xSpacing;
    //         float y = Mathf.Min((value / maxDataValue) * graphHeight, graphHeight); // Scale Y value to graph height
    //         graphPositions[index] = new Vector3(x, y, 0);
    //         index++;
    //     }

    //     lineRenderer.positionCount = graphPositions.Length;
    //     lineRenderer.SetPositions(graphPositions);

    //     Debug.Log($"Graph updated with {graphPositions.Length} points.");
    // }
    private void UpdateGraph()
    {
        if (lineRenderer == null) return;

        // 최대 데이터 포인트 개수에 따른 X축 간격
        float xSpacing = graphWidth / (maxPoints - 1);

        // 그래프 포지션 초기화
        Vector3[] graphPositions = new Vector3[dataPoints.Count];
        int index = 0;

        foreach (float value in dataPoints)
        {
            // X축 값 계산
            float x = index * xSpacing;

            // Y축 값은 최대값 제한 (graphHeight의 0.8을 넘지 않도록 제한)
            float y = Mathf.Min((value / maxDataValue) * (graphHeight * 0.8f), graphHeight * 0.8f);

            // 그래프 포인트 저장
            graphPositions[index] = new Vector3(x, y, 0);
            index++;
        }

        // LineRenderer 업데이트
        lineRenderer.positionCount = graphPositions.Length;
        lineRenderer.SetPositions(graphPositions);

        Debug.Log($"Graph updated with {graphPositions.Length} points.");
    }


    private void DrawThreshold()
    {
        if (thresholdLine == null)
        {
            Debug.LogError("ThresholdLine is not assigned!");
            return;
        }
        else
        {
            Debug.LogError("ThresholdLine is assigned!");
        }

        // Threshold Y 위치 계산
        float scaledThresholdY = Mathf.Clamp(
            (emgThreshold / maxDataValue) * graphHeight, 
            0, 
            graphHeight * 0.8f
        );

        // Threshold LineRenderer 설정
        thresholdLine.positionCount = 2; // 2개의 점으로 구성
        thresholdLine.useWorldSpace = false; // 그래프 내부 좌표를 사용

        thresholdLine.SetPosition(0, new Vector3(0, scaledThresholdY, 0)); // 왼쪽 끝
        thresholdLine.SetPosition(1, new Vector3(graphWidth, scaledThresholdY, 0)); // 오른쪽 끝

        Debug.Log($"Threshold line drawn at Y={scaledThresholdY}");
    }

    private void FinalizePacking(bool success)
    {
        isPackingActive = false;

        if (success)
        {
            Debug.Log("Packing Success!");
            if (successMessage != null) successMessage.SetActive(true);
            HideGraphElements();

        // 모션을 시작
            StartCoroutine(WaitAndStartBurgerMotion(2f));
        }
        else
        {
            Debug.Log("Packing Failed.");
            if (failMessage != null) failMessage.SetActive(true);
            HideGraphElements();
            StartCoroutine(WaitAndShowGameOverPanel(5f));
        }
    }
    private IEnumerator WaitAndStartBurgerMotion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // 대기
        StartBurgerMotion(); // 모션 시작
        Debug.Log("Burger motion started after waiting.");
        StartCoroutine(WaitAndShowGameOverPanel(5f));
    }

    private IEnumerator WaitAndShowGameOverPanel(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // 대기
        ShowGameOverPanel(); // Game Over 패널 활성화
        Debug.Log("Game Over Panel shown after motion.");
    }

    private void HideGraphElements()
    {
        // GraphCanvas 안의 그래프 요소들을 비활성화
        if (GraphBackground != null) GraphBackground.gameObject.SetActive(false);
        if (thresholdLine != null) thresholdLine.gameObject.SetActive(false);
        if (lineRenderer != null) lineRenderer.gameObject.SetActive(false);
        Debug.Log("Graph elements hidden.");
    }

    private void StartBurgerMotion()
    {
        isMoving = true; // 모션 시작 플래그 설정
        Debug.Log("Burger motion started.");
    }
    private void ScaleGraphToBackground()
    {
        if (GraphBackground != null)
        {
            float width = GraphBackground.rect.width;
            float height = GraphBackground.rect.height;

            xSpacing = graphWidth / maxPoints; // Adjust xSpacing based on width
            yScale = graphHeight / maxDataValue; // Adjust yScale based on height

            Debug.Log($"Graph scaled: Width={graphWidth}, Height={graphHeight}, XSpacing={xSpacing}, YScale={yScale}");
        }
    }            
    private void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Game Over Panel Activated.");
        }
        else
        {
            Debug.LogWarning("GameOverPanel is not assigned!");
        }
    }
}
