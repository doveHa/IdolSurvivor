using System.Collections.Generic;
using Script.Characters;
using Script.DataDefinition.Enum;
using Script.Manager;
using Script.UI.DragDrop;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Script.TeamBuilding
{
    public class TeamBuildingManager : ManagerBase<TeamBuildingManager>
    {
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private GameObject leaderObject;
        [SerializeField] private GameObject followerObject;

        [SerializeField] private TextMeshProUGUI teamAllStat;
        [SerializeField] private TextMeshProUGUI teamColorText;

        [SerializeField] private GameObject nextSceneButton;
        private int currentMaking { get; set; } = 1;
        public Team[] teams; //private -> public
        public Team PlayerTeam { get; private set; }

        private List<GameObject> remainCards;

        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != Constant.Scene.TEAM_BUILDING) return;

            int leaderIndex = 0;
            teams = new Team[Config.Team.TeamCount];

            PlayerTeam = new Team(AllCharacterManager.Manager.Player);
            teams[leaderIndex++] = PlayerTeam;

            Destroy(AddCard(AllCharacterManager.Manager.Player, leaderObject).GetComponent<DraggableObject>());
            remainCards = new List<GameObject>();

            for (int index = 0; index < Config.Team.AllCharacterCount; index++)
            {
                Character character = AllCharacterManager.Manager.Ranks[index];

                if (character == AllCharacterManager.Manager.Player)
                    continue;

                if (leaderIndex < Config.Team.TeamCount)
                {
                    GameObject card = AddCard(character, leaderObject);
                    GameObject slot = card.GetComponentInChildren<DroppableObject>().gameObject;
                    teams[leaderIndex] = new Team(character);
                    teams[leaderIndex].SetSlot(slot);
                    Destroy(slot.GetComponent<DroppableObject>());
                    Destroy(card.GetComponent<DraggableObject>());
                    leaderIndex++;
                }
                else
                {
                    GameObject card = AddCard(character, followerObject);
                    Destroy(card.GetComponentInChildren<DroppableObject>().gameObject);
                    remainCards.Add(card);
                }
            }

            ShowTeamStat();
        }

        public void ShowTeamStat()
        {
            teamAllStat.text = PlayerTeam.TotalStat().ToString();
        }

        public CharacterStats ShowAndGetPredictionTeamStat(CharacterStats stat)
        {
            CharacterStats predictionTeamStat = PlayerTeam.PredictionAddStat(stat);
            teamAllStat.text = predictionTeamStat.ToString();
            return predictionTeamStat;
        }

        public void AddPlayerTeam(Character characterCard)
        {
            PlayerTeam.AddTeamMate(characterCard);
            currentMaking++;

            RandomTeammateAdd();
            if (currentMaking >= Constant.Team.MAX_MEMBER)
            {
                TeamBuildingEnd();
            }
        }

        public void UseCharacterCard(GameObject characterCard)
        {
            remainCards.Remove(characterCard);
        }

        public void ShowTeamColor(TeamColor teamColor)
        {
            if (teamColor != TeamColor.None)
            {
                teamColorText.text = teamColor.ToString();
            }
            else
            {
                teamColorText.text = "";
            }
        }

        public void AdjustResult()
        {
            foreach (Team team in teams)
            {
                float performance = team.CalculateTotalPerformance();
                foreach (Character character in team.Teams)
                {
                    character.AddVote(Mathf.RoundToInt(character.VoteRatio * performance));
                }
            }
        }

        private GameObject AddCard(Character character, GameObject slot)
        {
            CharacterCardHandler cardHandler = Instantiate(characterCardPrefab, slot.transform)
                .GetComponent<CharacterCardHandler>();
            cardHandler.SetCharacter(character);
            return cardHandler.gameObject;
        }

        private void RandomTeammateAdd()
        {
            for (int i = 1; i < teams.Length; i++)
            {
                Team team = teams[i];
                int randomIndex = Random.Range(0, remainCards.Count);
                team.AddTeamMate(remainCards[randomIndex].GetComponent<CharacterCardHandler>().Character);
                remainCards[randomIndex].transform.SetParent(team.teammateSlot.transform);
                Destroy(remainCards[randomIndex].GetComponent<DraggableObject>());
                remainCards.RemoveAt(randomIndex);
            }
        }

        private void TeamBuildingEnd()
        {
            Debug.Log("Team building end");
            nextSceneButton.SetActive(true);
        }
    }
}
