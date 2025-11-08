using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum DiceCheckResult
{
    CriticalSuccess,
    Success,
    Normal,
    Failure
}

namespace Script
{
    public class DiceRoller : MonoBehaviour
    {
        public static DiceRoller Instance { get; private set; }

        private Action<int> onRollFinishedCallback;

        public static int lastRollResult = 0;

        [Header("UI Component")]
        public GameObject dicePanel;
        public Image diceImage;
        public TMPro.TextMeshProUGUI resultText;
        public Button rollButton;
        private TMPro.TextMeshProUGUI rollButtonText;

        [Header("Dice Sprites")] public Sprite[] diceSprites;

        [Header("Animation Settings")] public int diceSides = 6;
        public float rollDuration = 0.5f;
        public float changeInterval = 0.05f;

        public Action onNextAction;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // 씬이 전환되어도 유지하고 싶다면 DontDestroyOnLoad(gameObject);를 추가합니다.
                // 하지만 주사위 UI는 보통 해당 씬에서만 필요하므로 제거하는 것을 권장합니다.
            }
            else
            {
                Destroy(gameObject);
            }

            // 시작 시 패널 비활성화
            if (dicePanel != null)
            {
                dicePanel.SetActive(false);
            }

            if (rollButton != null)
            {
                rollButtonText = rollButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            }
        }

        public void Start()
        {
            HideDicePanel();
        }

        public void ShowDicePanel()
        {
            if (dicePanel != null)
            {
                dicePanel.SetActive(true);
                resultText.text = "버튼을 눌러 주사위를 굴리세요.";
            }
        }

        public void HideDicePanel()
        {
            if (dicePanel != null)
            {
                dicePanel.SetActive(false);
            }
        }

        public void OnNextButtonClicked()
        {
            //HideDicePanel();

            if (rollButtonText != null)
            {
                rollButtonText.text = "Roll";
            }

            // 3. Next 버튼이 눌린 후 실행될 외부 로직이 있다면 호출
            onNextAction?.Invoke();
            onNextAction = null; // 실행 후 초기화 (다음 굴림을 위해)
        }

        public void SetRollCompletedUI()
        {
            if (rollButtonText != null)
            {
                rollButtonText.text = "Next";
            }

            if (rollButton != null)
            {
                rollButton.onClick.RemoveAllListeners();
                rollButton.onClick.AddListener(OnNextButtonClicked);
                rollButton.interactable = true;
            }
        }

        public static void RollDice(int diceSides)
        {
            int result = UnityEngine.Random.Range(1, diceSides + 1);

            lastRollResult = result;

            Debug.Log($"D{diceSides} �ֻ��� ���: {result}");
        }

        public void RollDiceWithAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(AnimateDiceRoll());
        }

        IEnumerator AnimateDiceRoll()
        {
            if (diceSprites == null || diceSprites.Length != 6 || diceImage == null)
            {
                Debug.LogError("�ֻ��� ��������Ʈ �迭�� 6������ �ϸ�, Image ������Ʈ�� �Ҵ�Ǿ�� �մϴ�.");
                yield break;
            }

            float startTime = Time.time;

            while (Time.time < startTime + rollDuration)
            {
                int randomSpriteIndex = UnityEngine.Random.Range(0, diceSprites.Length);
                diceImage.sprite = diceSprites[randomSpriteIndex];

                yield return new WaitForSeconds(changeInterval);
            }

            int result = UnityEngine.Random.Range(1, diceSides + 1);
            lastRollResult = result;

            int finalSpriteIndex = result - 1;

            diceImage.sprite = diceSprites[finalSpriteIndex];
            resultText.text = $"결과: {result}";

            Debug.Log($"D{diceSides} DiceRollResult: {result}");

            if (onRollFinishedCallback != null)
            {
                onRollFinishedCallback.Invoke(result); // <--- 콜백 실행은 여기서만!
            }
        }

        public void RollDiceWithCallback(Action<int> callback)
        {
            if (dicePanel != null && !dicePanel.activeSelf)
            {
                // 패널이 닫혀있다면, 열어주는 것이 좋습니다.
                ShowDicePanel();
            }

            onRollFinishedCallback = callback;

            StopAllCoroutines();
            StartCoroutine(AnimateDiceRoll());
        }
    }
}