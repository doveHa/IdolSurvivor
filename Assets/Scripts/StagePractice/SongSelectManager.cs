using Script;
using Script.Manager;
using Script.TeamBuilding;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongSelectManager : MonoBehaviour
{
    public GameObject SongPanel;
    public SongData[] availableSongs = new SongData[4];
    private SongData currentSelectedSong;

    public void Start()
    {
        GMManager.Instance.gmPanel.SetActive(true);

        string[] initialExplanation = new string[]
        {
            "이제 경연 곡을 선택할 시간입니다.",
            "각 곡은 콘셉트에 맞는 효과 스탯과 비효과 스탯을 가지고 있습니다.",
            "주사위 결과에 따라 노래의 스탯 적용이 달라집니다."
        };

        if (GMManager.Instance != null)
        {
            // 설명이 모두 끝난 후 실행될 콜백: ShowSongPanel 함수
            GMManager.Instance.StartDialogue(initialExplanation, ShowSongPanel);
        }

        SongPanel.SetActive(false);

        //Debug.Log(AllCharacterManager.Manager.Player.Stat.ToString());
        //Debug.Log(TeamBuildingManager.Manager.PlayerTeam.TotalStat());   
    }

    // 콜백
    private void ShowSongPanel()
    {
        SongPanel.SetActive(true);
    }

    public void OnClickSong(int songIndex)
    {
        if (songIndex < 0 || songIndex >= availableSongs.Length)
        {
            Debug.LogError($"잘못된 노래 인덱스 ({songIndex})가 전달되었습니다.");
            return;
        }

        SongData selectedSong = availableSongs[songIndex];
        currentSelectedSong = selectedSong;

        Debug.Log($"선택된 노래: {selectedSong.songName}, 콘셉트: {selectedSong.concept}, 감소스탯: {selectedSong.decreaseStat}, 증가스탯: {selectedSong.increaseStat}");

        if (GMManager.Instance != null)
        {
            GMManager.Instance.gmText.text = $"'{selectedSong.songName}'을(를) 선택하시겠습니까?\n";
            GMManager.Instance.gmPanel.SetActive(true);

            //씬마다 다른 판단 범위
            GMManager.Instance.NextBtn.onClick.RemoveAllListeners(); // 기존 리스너 삭제 (중복 방지)
            GMManager.Instance.NextBtn.onClick.AddListener(() => OnConfirmRoll());
        }
    }

    private void OnConfirmRoll()
    {
        // GMManager의 대화창 닫기
        GMManager.Instance.gmPanel.SetActive(false);

        SongPanel.SetActive(false);

        if (Script.DiceRoller.Instance != null)
        {
            Script.DiceRoller.Instance.ShowDicePanel();

            if (Script.DiceRoller.Instance.rollButton != null)
            {
                Button rollBtn = Script.DiceRoller.Instance.rollButton;
                rollBtn.onClick.RemoveAllListeners(); //기존 리스너 삭제

                rollBtn.onClick.AddListener(() =>
                {
                    Script.DiceRoller.Instance.RollDiceWithCallback(SongJudgeResult); //노래용 판정
                    rollBtn.interactable = false; //다시 굴리기 방지
                });
            }
        }
    }

    private void SongJudgeResult(int roll)
    {
        // 1. 노래 선택 상황에 맞는 판단 로직 실행
        DiceCheckResult checkResult = JudgeRollResultForSong(roll);

        // 2. 판단 결과에 따라 스탯 적용 및 메시지 출력 (GMManager나 DiceRoller의 resultText 사용)
        string resultMessage = ProcessSongEffect(checkResult, roll);

        if (Script.DiceRoller.Instance != null)
        {
            Script.DiceRoller.Instance.resultText.text = resultMessage;

            Script.DiceRoller.Instance.SetRollCompletedUI();

            // Next 버튼 -> 포지션 및 파트 배분으로 이동
            Script.DiceRoller.Instance.onNextAction = () =>
            {
                SceneManager.LoadScene("PositionPart");
            };
        }
    }

    private DiceCheckResult JudgeRollResultForSong(int roll)
    {
        if (roll == 6)
        {
            Debug.Log($"{currentSelectedSong.decreaseStat} 스탯이 제거 예정.");
            return DiceCheckResult.CriticalSuccess; // 대성공
        }
        else if (roll == 4 || roll == 5)
        {
            Debug.Log($"{currentSelectedSong.songName} 진행 예정.");
            return DiceCheckResult.Success; // 성공
        }
        else // roll == 1, 2, 3
        {
            Debug.Log($"{currentSelectedSong.increaseStat} 스탯이 제거 예정.");
            return DiceCheckResult.Failure; // 실패
        }
    }

    private string ProcessSongEffect(DiceCheckResult result, int roll)
    {
        if (result == DiceCheckResult.CriticalSuccess)
            return $"[D{roll}] 대성공! \n콘셉트 감소 효과가 사라집니다.";

        if (result == DiceCheckResult.Success)
            return $"[D{roll}] 성공!\n증가와 감소 효과가 유지됩니다.";

        if (result == DiceCheckResult.Failure)
            return $"[D{roll}] 실패...\n증가 효과가 사라집니다.";

        return "판정 오류";
    }
}
