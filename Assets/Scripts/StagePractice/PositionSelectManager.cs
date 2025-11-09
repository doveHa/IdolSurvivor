using Script;
using Script.DataDefinition.Enum;
using Script.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Script.Characters;
using Script.TeamBuilding;


public class PositionSelectManager : MonoBehaviour
{
    private PositionResultData playerPositionResult;
    private LineDistributionData playerDistributionResult;

    //private PositionResultData positionResult;
    //private LineDistributionData distributionResult;

    // 포지션 결정에 필요한 데이터 목록
    private PositionResultData[] positionData = new PositionResultData[]
    {
        new PositionResultData { checkResult = DiceCheckResult.CriticalSuccess, positionName = "센터", voteRatio = 0.5f },
        new PositionResultData { checkResult = DiceCheckResult.Success, positionName = "메보", voteRatio = 0.3f },
        new PositionResultData { checkResult = DiceCheckResult.Failure, positionName = "서브/래퍼", voteRatio = 0.1f }
    };

    // 분량 결정에 필요한 데이터 목록 (주사위 결과 3(Normal)을 중간으로 사용)
    private LineDistributionData[] distributionData = new LineDistributionData[]
    {
        new LineDistributionData { checkResult = DiceCheckResult.CriticalSuccess, distributionLabel = "많음", eventCount = 5 },
        new LineDistributionData { checkResult = DiceCheckResult.Success, distributionLabel = "중간", eventCount = 3 },
        new LineDistributionData { checkResult = DiceCheckResult.Failure, distributionLabel = "적음", eventCount = 1 }
    };

    void Start()
    {
        StartInitialDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartInitialDialogue()
    {
        string[] initialExplanation = new string[]
        {
            "이제 포지션을 정해볼까?",
            "혹시 센터 하고 싶은 사람 있어?",
            "...",
            "모두 센터를 원하고 있네.",
            "그럼 주사위를 굴려서 포지션을 정해볼까?"
        };

        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 포지션 주사위를 굴리도록 콜백 연결
            GMManager.Instance.StartDialogue(initialExplanation, RollPositionDice);
        }
    }

    private void RollPositionDice()
    {
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                rollBtn.onClick.RemoveAllListeners(); 

                rollBtn.onClick.AddListener(() =>
                {
                    DiceRoller.Instance.RollDiceWithCallback(ProcessPositionResult);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessPositionResult(int roll)
    {
        DiceCheckResult check = JudgeRollResult(roll);

        playerPositionResult = Array.Find(positionData, d => d.checkResult == check);

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text =
                $"[D{roll}] {check}!\n포지션은 {playerPositionResult.positionName}으로 결정되었습니다.";

            DiceRoller.Instance.SetRollCompletedUI();
            //DiceRoller.Instance.onNextAction = StartDistributionDialogue;
            DiceRoller.Instance.onNextAction = StartDistributionDialogue;
        }

        Debug.Log($"포지션 결정: {playerPositionResult.positionName}, 득표 비율: {playerPositionResult.voteRatio}");
    }

    private void StartDistributionDialogue()
    {
        string[] dialogue = new string[]
        {
            "포지션을 정했으니 이제 파트를 나눠보자.",
            "주사위를 굴려 이번 경연에서 받을 분량(이벤트 수)을 정합니다."
        };

        if (GMManager.Instance != null)
        {
            // 대화가 끝난 후 분량 주사위를 굴리도록 콜백 연결
            GMManager.Instance.StartDialogue(dialogue, RollDistributionDice);
        }
    }

    private void RollDistributionDice()
    {
        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.ShowDicePanel();

            Button rollBtn = DiceRoller.Instance.rollButton;
            if (rollBtn != null)
            {
                rollBtn.onClick.RemoveAllListeners();

                rollBtn.onClick.AddListener(() =>
                {
                    DiceRoller.Instance.RollDiceWithCallback(ProcessDistributionResult);
                    rollBtn.interactable = false;
                });

                rollBtn.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Roll";
                rollBtn.interactable = true;
            }
        }
    }

    private void ProcessDistributionResult(int roll)
    {
        DiceCheckResult check = JudgeRollResult(roll);

        playerDistributionResult = Array.Find(distributionData, d => d.checkResult == check);

        if (DiceRoller.Instance != null)
        {
            DiceRoller.Instance.resultText.text =
                $"[D{roll}] {check}!\n분량은 {playerDistributionResult.distributionLabel} ({playerDistributionResult.eventCount}개)로 결정되었습니다.";

            DiceRoller.Instance.SetRollCompletedUI();

            //팀원 포지션
            DiceRoller.Instance.onNextAction = FinalizeTeamAndLoadScene;

/*            DiceRoller.Instance.onNextAction = () =>
            {
                Debug.Log("모든 결정 완료! 다음 단계로...");
                SceneManager.LoadScene("Practice");
            };*/
        }

        Debug.Log($"분량 결정: {playerDistributionResult.distributionLabel}, 이벤트 수: {playerDistributionResult.eventCount}");

        // 공연에 이벤트 수 반영
        Config.Event.EventCount = playerDistributionResult.eventCount;
        Debug.Log($"Config.Event.EventCount가 {Config.Event.EventCount}로 설정되었습니다.");
    }

    private DiceCheckResult JudgeRollResult(int roll)
    {
        if (roll == 6) return DiceCheckResult.CriticalSuccess;
        if (roll == 4 || roll == 5) return DiceCheckResult.Success;
        return DiceCheckResult.Failure;
    }

    private void FinalizeTeamAndLoadScene()
    {
        if (TeamBuildingManager.Manager.PlayerTeam.Teams.Count < 3)
        {
            Debug.LogError("팀 빌딩이 완료되지 않았습니다. 팀원 수가 3명이 아닙니다.");
            // 팀 빌딩이 완료되지 않은 경우를 위한 예외 처리 필요
            return;
        }

        DetermineTeamPositions(TeamBuildingManager.Manager.PlayerTeam);
        DetermineAllTeamsPositions();

        Debug.Log("모든 결정 완료! 다음 단계로...");
        // 씬 로드
        SceneManager.LoadScene("Practice");
    }

    private void DetermineTeamPositions(Team playerTeam)
    {
        List<string> availablePositions = positionData.Select(d => d.positionName).ToList();

        // 플레이어
        Character playerChar = playerTeam.Teams[0];
        playerChar.PositionName = playerPositionResult.positionName;
        playerChar.VoteRatio = playerPositionResult.voteRatio;

        availablePositions.Remove(playerPositionResult.positionName);

        // 나머지 팀원
        List<Character> otherTeammates = playerTeam.Teams.Skip(1).Take(2).ToList();
        System.Random rnd = new System.Random();

        foreach (Character teammate in otherTeammates)
        {
            int randomIndex = rnd.Next(0, availablePositions.Count);
            string assignedPosName = availablePositions[randomIndex];
            availablePositions.RemoveAt(randomIndex);

            PositionResultData assignedPosData = Array.Find(positionData, d => d.positionName == assignedPosName);

            teammate.PositionName = assignedPosData.positionName;
            teammate.VoteRatio = assignedPosData.voteRatio;

            Debug.Log($"팀원 포지션 결정: {teammate.Data.name} - {teammate.PositionName}");
        }

        Debug.Log("---우리 팀 포지션---");
        foreach (Character chara in playerTeam.Teams)
        {
            Debug.Log($"[{chara.Data.name}] 포지션: {chara.PositionName}, 투표 비율: {chara.VoteRatio}");
        }
    }

    private void DetermineAllTeamsPositions()
    {
        Team[] allTeams = TeamBuildingManager.Manager.teams;

        if (allTeams == null || allTeams.Length == 0)
        {
            Debug.LogWarning("TeamBuildingManager에서 팀 목록을 가져올 수 없거나, 팀이 구성되지 않았습니다.");
            return;
        }

        Debug.Log("------");
        foreach (Team team in allTeams)
        {
            if (team == TeamBuildingManager.Manager.PlayerTeam)
            {
                continue;
            }

            List<string> remainingPositions = positionData.Select(d => d.positionName).ToList();
            System.Random rnd = new System.Random();

            foreach (Character character in team.Teams)
            {
                // 팀원이 3명이라고 가정
                if (string.IsNullOrEmpty(character.PositionName))
                {
                    if (remainingPositions.Count == 0)
                    {
                        // 남은 포지션이 없으면 랜덤으로 전체 중에서 다시 하나 선택
                        remainingPositions = positionData.Select(d => d.positionName).ToList();
                    }

                    int randomIndex = rnd.Next(0, remainingPositions.Count);
                    string assignedPosName = remainingPositions[randomIndex];
                    remainingPositions.RemoveAt(randomIndex);

                    PositionResultData assignedPosData = Array.Find(positionData, d => d.positionName == assignedPosName);

                    character.PositionName = assignedPosData.positionName;
                    character.VoteRatio = assignedPosData.voteRatio;

                    Debug.Log($"캐릭터: {character.Data.name} 포지션: {character.PositionName} 투표 비율:{character.VoteRatio}");
                }
            }
        }
        Debug.Log("------");
    }
}
