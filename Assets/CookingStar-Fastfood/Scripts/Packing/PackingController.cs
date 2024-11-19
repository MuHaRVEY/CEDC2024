using UnityEngine;

public class BurgerPacking : MonoBehaviour
{
    public Transform burgerForPacking; // 움직이는 햄버거
    public Transform pack; // 목표 위치 (포장지)
    public float moveSpeed = 5f; // 이동 속도

    private bool isMoving = false; // 이동 상태를 체크

    void Update()
    {
        // 스페이스바를 누르면 이동 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isMoving = true;
        }

        // 이동 중이면 Lerp로 위치를 업데이트
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
                burgerForPacking.position = pack.position; // 정확히 포장지 위치로 설정
                burgerForPacking.localScale = Vector3.zero; // 크기를 0으로 설정 (사라지듯 연출)
                Debug.Log("Burger reached the pack!");
            }
        }
    }
}
