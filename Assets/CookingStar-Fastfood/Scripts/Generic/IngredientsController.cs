// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;

// namespace CookingStar
// {
//     public class IngredientsController : MonoBehaviour
//     {


//         //테스트용 stt
//         //  private GoogleSpeechController googleSpeechController;

//         // void Start()
//         // {
//         //     // Google Speech Controller 초기화
//         //     googleSpeechController = GetComponent<GoogleSpeechController>();
//         // }

//         // 마이크 테스트 해보던 코드
//         // void Update()
//         // {
//         //     if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바로 테스트
//         //     {
//         //         googleSpeechController.StartRecognition(); 
//         //     }
//         // 	// 기존 재료 관리 로직
//         // 	ManagePlayerDrag();
//         // 	if (Input.touches.Length < 1 && !Input.GetMouseButton(0)) //원래 있던 코드 여기로 옮김
//         // 	{
//         // 		itemIsInHand = false;
//         // 	}
//         // }

//         public void HandleRecognizedText(string text)
//         {
//             if (text.Contains("lettuce"))
//             {
//                 Debug.Log("Adding lettuce!");
//                 // 재료 추가 로직
//             }
//             else if (text.Contains("tomato"))
//             {
//                 Debug.Log("Adding tomato!");
//                 // 재료 추가 로직
//             }
//         }
//         // 테스트용 stt fin
//         /// <summary>
//         /// Main class for Handling all things related to ingredients
//         /// </summary>

//         //public list of all available ingredients.
//         public GameObject[] ingredientsArray; // 모든 재료 배열
//         public Dictionary<string, string> ingredientWords = new Dictionary<string, string>(); // 재료와 단어 매칭
//         public static bool itemIsInHand; // 재료가 선택되었는지 여부
//         public BuController buController; // UIController 참조
//         public GameObject serverPlate; // 서버 접시 (serverPlate-1)
//         public float stackingOffset = 0.1f; // 재료가 쌓이는 간격 (Y축 간격)
//         private int stackCount = 1; // 현재 접시에 쌓인 재료의 개수

//         IEnumerator ManagePlayerDrag()
// 		{
// 			Ray ray; // Ray 변수 선언
//             RaycastHit hitInfo; // RaycastHit 변수 선언
//             if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Moved)
// 				ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
// 			else if (Input.GetMouseButtonDown(0))
// 				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
// 			else
// 				yield break;

// 			if (Physics.Raycast(ray, out hitInfo))
// 			{
// 				GameObject objectHit = hitInfo.transform.gameObject;
// 				if (objectHit.tag == "sideRequest" && objectHit.name == gameObject.name && !IngredientsController.itemIsInHand)
// 				{

// 					if (!needsProcess)
// 						SelectIngredient();
// 					else
// 						SelectIngredient(); //raw side request needs to be processed before use
// 				}
// 			}
// 		}
//         void Start()
//         {
//             Debug.Log("IngredientsController Start executed!");
//            //
//             ManagePlayerDrag();
//         	if (Input.touches.Length < 1 && !Input.GetMouseButton(0)) //원래 있던 코드 여기로 옮김
//         	{
//         		itemIsInHand = false;
//         	}
//             //
//             // itemIsInHand = false; // 초기 상태

//             if (buController != null)
//             {
//                 buController.ShowRecordButton();
//                 Debug.Log("ShowRecordButton() successfully called.");
//             }
//             else
//             {
//                Debug.LogError("BuController is not assigned in IngredientsController!");
//             }

//             // 명시적으로 재료와 특정 단어 매핑
//             ingredientWords.Add("Ingredient-Type-01", "안녕하세요");
//             ingredientWords.Add("Ingredient-Type-02", "고기");
//             ingredientWords.Add("Ingredient-Type-03", "오이");
//             ingredientWords.Add("Ingredient-Type-04", "토마토");
//             ingredientWords.Add("Ingredient-Type-05", "치즈");
//             ingredientWords.Add("Ingredient-Type-06", "햄");
//             ingredientWords.Add("Ingredient-Type-07", "케찹");
//             ingredientWords.Add("Ingredient-Type-08", "계란");
//             ingredientWords.Add("Ingredient-Type-09", "양파");
//             ingredientWords.Add("Ingredient-Type-10", "빵빵");
//             ingredientWords.Add("Ingredient-Type-11", "상추");
//             ingredientWords.Add("Ingredient-Type-12", "머스타드");
//             ingredientWords.Add("Ingredient-Type-13", "피클");
//             ingredientWords.Add("Ingredient-Type-14", "muya");
//             ingredientWords.Add("Ingredient-Type-15", "gamja");
//         }

//         public void SelectIngredientByWord(string recognizedWord)
//         {
//             Debug.Log($"SelectIngredientByWord called with word: {recognizedWord}");
//             foreach (var entry in ingredientWords)
//             {
//                 recognizedWord = recognizedWord.Trim().ToLower();

//                 if (recognizedWord.Equals(entry.Value)) // 변환된 단어와 사전의 Value를 비교
//                 {
//                     Debug.Log($"Matched ingredient: {entry.Value}");
//                     SelectIngredient(entry.Key); // 매칭된 Key로 재료 선택
//                     break;
//                 }
//             }
//             Debug.LogError($"No matching ingredient for recognized word: {recognizedWord}");
//         }

//         void SelectIngredient(string ingredientName)
//         {
//             if (serverPlate == null)
//             {
//                 Debug.LogError("ServerPlate is null. Make sure it is assigned in the Inspector!");
//                 return;
//             }

//             foreach (var ingredient in ingredientsArray)
//             {
//                 if (ingredient.name.Equals(ingredientName))
//                 {
//                     Debug.Log($"Matched ingredient: {ingredient.name}");

//                     // 재료 인스턴스 생성
//                     GameObject newIngredient = Instantiate(ingredient);

//                     if (newIngredient == null)
//                     {
//                         Debug.LogError("Failed to instantiate ingredient!");
//                         return;
//                     }

//                     // 재료의 기본 Transform 설정
//                     newIngredient.name = ingredient.name;
//                     newIngredient.tag = "deliveryQueueItem";

//                     // 재료의 위치를 serverPlate를 기준으로 설정
//                     newIngredient.transform.position = serverPlate.transform.position + new Vector3(
//                         0, // X축: 접시 중심
//                         stackCount * stackingOffset, // Y축: 쌓이는 간격
//                         0  // Z축: 접시 중심
//                     );

//                     // 재료의 회전 설정 (원래 코드와 동일)
//                     newIngredient.transform.rotation = Quaternion.Euler(90, 180, 0);

//                     // 재료의 크기 설정 (원래 코드와 동일)
//                     newIngredient.transform.localScale = new Vector3(0.085f, 0.01f, 0.05f);

//                     Debug.Log($"Ingredient {newIngredient.name} added to serverPlate at position: {newIngredient.transform.position}");

//                     // 스택 카운트 증가
//                     stackCount++;
//                     return;
//                 }
//             }
//             Debug.LogError($"Ingredient with name {ingredientName} not found in ingredientsArray!");
//         }
//     }
// }


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CookingStar
{
    public class IngredientsController : MonoBehaviour
    {
        public GameObject[] ingredientsArray; // 모든 재료 배열
        public Dictionary<string, string> ingredientWords = new Dictionary<string, string>(); // 재료와 단어 매핑
        public static bool itemIsInHand; // 재료가 선택되었는지 여부
        public BuController buController; // UIController 참조
        public GameObject serverPlate; // 서버 접시 (serverPlate-1)
        public float stackingOffset = 0.1f; // 재료가 쌓이는 간격 (Y축 간격)
        private int stackCount = 1; // 현재 접시에 쌓인 재료의 개수

        public UnityEngine.UI.Button giveButton; // 햄버거 건네기 버튼

        private List<GameObject> currentIngredients = new List<GameObject>(); // 서버 접시에 있는 현재 재료들

        private float delayTime;   
        private RaycastHit hitInfo;
      private Ray ray;
        public bool needsProcess = false;
        private bool canCreate = true; 
        public int factoryID;
        public float Zoffset = -1f; 
        public string processorTag = ""; 
        void Awake()
      {
         delayTime = 0.25f; //Default = 1f
         itemIsInHand = false;
      }
        void Update()
        {
            ManagePlayerDrag();

            if (Input.touches.Length < 1 && !Input.GetMouseButton(0))
            {
                itemIsInHand = false;
            }

            //debug
            //print ("itemIsInHand: " + itemIsInHand);
        }
        void Start()
        {
            Debug.Log("IngredientsController Start executed!");
            itemIsInHand = false; // 초기 상태

            if (buController != null)
            {
                buController.ShowRecordButton();
                Debug.Log("ShowRecordButton() successfully called.");
            }
            else
            {
                Debug.LogError("BuController is not assigned in IngredientsController!");
            }

            if (giveButton != null)
            {
                giveButton.onClick.AddListener(GiveBurger);
                Debug.Log("GiveButton is assigned and listener added.");
            }
            else
            {
                Debug.LogError("GiveButton is not assigned in the Inspector!");
            }


            // 명시적으로 재료와 특정 단어 매핑
            ingredientWords.Add("Ingredient-Type-01", "안녕하세요");
            ingredientWords.Add("Ingredient-Type-02", "고기");
            ingredientWords.Add("Ingredient-Type-03", "오이");
            ingredientWords.Add("Ingredient-Type-04", "토마토");
            ingredientWords.Add("Ingredient-Type-05", "치즈");
            ingredientWords.Add("Ingredient-Type-06", "햄");
            ingredientWords.Add("Ingredient-Type-07", "케찹");
            ingredientWords.Add("Ingredient-Type-08", "계란");
            ingredientWords.Add("Ingredient-Type-09", "양파");
            ingredientWords.Add("Ingredient-Type-10", "빵빵");
            ingredientWords.Add("Ingredient-Type-11", "상추");
            ingredientWords.Add("Ingredient-Type-12", "머스타드");
            ingredientWords.Add("Ingredient-Type-13", "피클");
            ingredientWords.Add("Ingredient-Type-14", "muya");
            ingredientWords.Add("Ingredient-Type-15", "gamja");
        }

        public void GiveBurger()
        {
            // 접시 위의 재료를 가져옴
            List<int> deliveredOrder = new List<int>();
            foreach (var ingredient in currentIngredients)
            {
                if (ingredient != null)
                {
                    var productManager = ingredient.GetComponent<ProductManager>();
                    if (productManager != null)
                    {
                        deliveredOrder.AddRange(productManager.ingredientsIDs);
                    }
                }
            }

            // 접시 위의 재료들을 드래그 가능한 상태로 만듦 (CustomerController에 의존하지 않음)
            foreach (var ingredient in currentIngredients)
            {
                ingredient.tag = "deliveryQueueItem"; // 드래그 가능하도록 태그 변경
            }

            Debug.Log("Ingredients are ready to be served!");

            // 접시 초기화
            foreach (var ingredient in currentIngredients)
            {
                Destroy(ingredient);
            }
            currentIngredients.Clear();
            stackCount = 1;

            // 이후의 행동은 CustomerController에서 자동 처리
        }

        void ManagePlayerDrag()
      {
         //Mouse of touch?
         if (Input.touches.Length > 0 && Input.touches[0].phase == TouchPhase.Moved)
            ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
         else if (Input.GetMouseButtonDown(0))
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         else
            return;

         if (Physics.Raycast(ray, out hitInfo))
         {
            GameObject objectHit = hitInfo.transform.gameObject;
            if (objectHit.tag == "ingredient" && objectHit.name == gameObject.name && !itemIsInHand)
            {

               if (!needsProcess)
                  CreateIngredient();
               else
                  CreateRawIngredient();  //raw ingredient needs to be processed (in a machine) before use
            }
         }
      }
        void CreateIngredient()
      {
         if (canCreate && !MainGameController.gameIsFinished && MainGameController.gameIsStarted && !MainGameController.deliveryQueueIsFull)
         {
            canCreate = false;
            itemIsInHand = true;
            GameObject prod = Instantiate(ingredientsArray[factoryID - 1], transform.position + new Vector3(0, 0, Zoffset), Quaternion.Euler(90, 180, 0)) as GameObject;
            prod.name = ingredientsArray[factoryID - 1].name;
            prod.tag = "deliveryQueueItem";
            prod.GetComponent<MeshCollider>().enabled = false;
            prod.GetComponent<ProductMover>().factoryID = factoryID;
            prod.GetComponent<ProductMover>().needsProcess = false;
            prod.transform.localScale = new Vector3(0.085f, 0.01f, 0.05f); //hardcoded scale value. change with caution!
            SfxPlayer.instance.PlaySfx(5);
            StartCoroutine(Reactivate());
         }
      }
        void CreateRawIngredient()
      {
         if (canCreate && !MainGameController.gameIsFinished && MainGameController.gameIsStarted)
         {
            canCreate = false;
            itemIsInHand = true;
            GameObject prod = Instantiate(ingredientsArray[factoryID - 1], transform.position + new Vector3(0, 0, Zoffset), Quaternion.Euler(90, 180, 0)) as GameObject;
            prod.name = ingredientsArray[factoryID - 1].name + "-RAW";
            prod.tag = "rawIngredient";
            prod.GetComponent<ProductMover>().factoryID = factoryID;
            prod.GetComponent<ProductMover>().needsProcess = true;
            prod.GetComponent<ProductMover>().processorTag = processorTag;
            prod.transform.localScale = new Vector3(0.085f, 0.01f, 0.06f);
            SfxPlayer.instance.PlaySfx(5);
            StartCoroutine(Reactivate());
         }
      }

        public void SelectIngredientByWord(string recognizedWord)
        {
            Debug.Log($"SelectIngredientByWord called with word: {recognizedWord}");
            foreach (var entry in ingredientWords)
            {
                recognizedWord = recognizedWord.Trim().ToLower();

                if (recognizedWord.Equals(entry.Value)) // 변환된 단어와 사전의 Value를 비교
                {
                    Debug.Log($"Matched ingredient: {entry.Value}");
                    SelectIngredient(entry.Key); // 매칭된 Key로 재료 선택
                    return;
                }
            }
            Debug.LogError($"No matching ingredient for recognized word: {recognizedWord}");
        }

        void SelectIngredient(string ingredientName)
        {
            if (serverPlate == null)
            {
                Debug.LogError("ServerPlate is null. Make sure it is assigned in the Inspector!");
                return;
            }

            foreach (var ingredient in ingredientsArray)
            {
                if (ingredient.name.Equals(ingredientName))
                {
                    Debug.Log($"Matched ingredient: {ingredient.name}");

                    // 재료 인스턴스 생성
                    GameObject newIngredient = Instantiate(ingredient);

                    if (newIngredient == null)
                    {
                        Debug.LogError("Failed to instantiate ingredient!");
                        return;
                    }

                    // 재료의 기본 Transform 설정
                    newIngredient.name = ingredient.name;
                    newIngredient.tag = "deliveryQueueItem";

                    // 재료의 위치를 serverPlate를 기준으로 설정
                    newIngredient.transform.position = serverPlate.transform.position + new Vector3(
                        0, // X축: 접시 중심
                        stackCount * stackingOffset, // Y축: 쌓이는 간격
                        0  // Z축: 접시 중심
                    );

                    // 재료의 회전 설정 (원래 코드와 동일)
                    newIngredient.transform.rotation = Quaternion.Euler(90, 180, 0);

                    // 재료의 크기 설정 (원래 코드와 동일)
                    newIngredient.transform.localScale = new Vector3(0.085f, 0.01f, 0.05f);

                    Debug.Log($"Ingredient {newIngredient.name} added to serverPlate at position: {newIngredient.transform.position}");

                    // 서버 접시에 추가된 재료 저장
                    currentIngredients.Add(newIngredient);

                    // 스택 카운트 증가
                    stackCount++;
                    return;
                }
            }
            Debug.LogError($"Ingredient with name {ingredientName} not found in ingredientsArray!");
        }
        IEnumerator Reactivate()
      {
         yield return new WaitForSeconds(delayTime);
         canCreate = true;
      }
    }
   
}
