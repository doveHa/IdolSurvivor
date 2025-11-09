using Script;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GMManager : MonoBehaviour
{
    public static GMManager Instance { get; private set; }

    public GameObject gmPanel;
    public TMPro.TextMeshProUGUI gmNameText;
    public TMPro.TextMeshProUGUI gmText;
    public Button NextBtn;
    public Button PrevBtn;

    private string[] currentDialogue;
    private int dialogueIndex = 0;
    private Action onDialogueEndAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //gmPanel.SetActive(false);

/*        if (NextBtn != null)
        {
            NextBtn.onClick.AddListener(OnClickNext);
        }*/

        if (PrevBtn != null)
        {
            PrevBtn.onClick.AddListener(OnClickPrev);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*    public void OnClickNext()
        {
            Debug.Log("Next button clicked");

            gmPanel.SetActive(false);

            if (DiceRoller.Instance != null)
            {
                DiceRoller.Instance.ShowDicePanel();
            }
            else
            {
                Debug.LogError("DiceRoller instance is null.");
            }
        }*/

    public void OnClickNext()
    {
        if (currentDialogue != null && dialogueIndex < currentDialogue.Length - 1)
        {
            // 텍스트가 더 남아있으면 다음 텍스트 출력
            dialogueIndex++;
            UpdateDialogueText();
        }
        else
        {
            // 텍스트가 끝났거나, 현재 대화 상태가 아닌 경우
            gmPanel.SetActive(false);
            currentDialogue = null; // 대화 상태 초기화

            // 콜백 실행: 설명이 모두 끝났음을 외부에 알립니다.
            onDialogueEndAction?.Invoke();
            onDialogueEndAction = null;
        }
    }


    public void OnClickPrev()
    {
        Debug.Log("Previous button clicked");
        gmPanel.SetActive(false);
    }

    public void StartDialogue(string[] dialogue, Action onEnd = null)
    {
        if (dialogue == null || dialogue.Length == 0) return;

        currentDialogue = dialogue;
        dialogueIndex = 0;
        onDialogueEndAction = onEnd; // 설명 끝 콜백 저장

        gmPanel.SetActive(true);

        //NextBtn.gameObject.SetActive(true); // Next 버튼 활성화
        if (NextBtn != null)
        {
            NextBtn.onClick.RemoveAllListeners(); // 안전하게 기존 것 제거
            NextBtn.onClick.AddListener(OnClickNext); // 대화 진행 리스너 다시 연결
            NextBtn.gameObject.SetActive(true);
        }

        PrevBtn.gameObject.SetActive(false); // 이전 버튼 비활성화 (선택 사항)

        // 첫 번째 텍스트 출력
        UpdateDialogueText();
    }

    private void UpdateDialogueText()
    {
        if (gmText != null && currentDialogue != null && dialogueIndex < currentDialogue.Length)
        {
            gmText.text = currentDialogue[dialogueIndex];
        }
    }
}
