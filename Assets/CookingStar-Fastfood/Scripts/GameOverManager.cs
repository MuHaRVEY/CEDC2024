using UnityEngine;
using UnityEngine.UI;

namespace CookingStar
{
    public class GameOverManager : MonoBehaviour
    {
        public GameObject gameOverPanel; // GameOverPanel 오브젝트
        public Button recordButton; // Record 버튼
        public GameObject[] textObjects; // Text1~Text12까지의 텍스트 오브젝트 배열

        void Start()
        {
            // 초기 상태: GameOverPanel 비활성화
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }

        public void ActivateGameOver()
        {
            // GameOverPanel 활성화
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            // Record 버튼 비활성화
            if (recordButton != null)
                recordButton.gameObject.SetActive(false);

            // Text1~Text12 비활성화
            if (textObjects != null && textObjects.Length > 0)
            {
                foreach (var textObj in textObjects)
                {
                    if (textObj != null)
                        textObj.SetActive(false);
                }
            }
        }

        public void DeactivateGameOver()
        {
            // GameOverPanel 비활성화
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            // Record 버튼 활성화
            if (recordButton != null)
                recordButton.gameObject.SetActive(true);

            // Text1~Text12 활성화
            if (textObjects != null && textObjects.Length > 0)
            {
                foreach (var textObj in textObjects)
                {
                    if (textObj != null)
                        textObj.SetActive(true);
                }
            }
        }
    }
}
