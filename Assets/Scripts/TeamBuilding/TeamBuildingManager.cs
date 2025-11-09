using System.Collections.Generic;
using Script.Characters;
using Script.Manager;
using Script.UI.DragDrop;
using TMPro;
using UnityEngine;

namespace Script.TeamBuilding
{
    public class TeamBuildingManager : ManagerBase<TeamBuildingManager>
    {
        [SerializeField] private GameObject characterCardPrefab;
        [SerializeField] private GameObject leaderObject;
        [SerializeField] private GameObject followerObject;

        [SerializeField] private TextMeshProUGUI teamAllStat;

        private int currentMaking { get; set; } = 1;
        private Team[] teams;
        private Team playerTeam;

        private List<GameObject> remainCards;

        protected override void Awake()
        {
            base.Awake();
            teams = new Team[Config.Team.TeamCount];
        }

        void Start()
        {
            int leaderIndex = 0;
            playerTeam = new Team(AllCharacterManager.Manager.Player.Stat);
            teams[leaderIndex++] = playerTeam;
            Destroy(AddCard(AllCharacterManager.Manager.Player, leaderObject).GetComponent<DraggableObject>());
            remainCards = new List<GameObject>();

            for (int index = 0; index < Config.Team.AllCharacterCount; index++)
            {
                Character character = AllCharacterManager.Manager.Ranks[index];

                if (character == AllCharacterManager.Manager.Player)
                {
                    continue;
                }

                if (leaderIndex < Config.Team.TeamCount)
                {
                    GameObject card = AddCard(character, leaderObject);
                    GameObject slot = card.GetComponentInChildren<DroppableObject>().gameObject;
                    teams[leaderIndex] = new Team(character.Stat);
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
            teamAllStat.text = playerTeam.TotalStat().ToString();
        }

        public void ShowPredictionTeamStat(CharacterStats stat)
        {
            teamAllStat.text = playerTeam.PredictionAddStat(stat).ToString();
        }

        public void AddPlayerTeam(CharacterStats characterCard)
        {
            playerTeam.AddTeamMate(characterCard);
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
                team.AddTeamMate(remainCards[randomIndex].GetComponent<CharacterCardHandler>().Character.Stat);
                remainCards[randomIndex].transform.SetParent(team.teammateSlot.transform);
                Destroy(remainCards[randomIndex].GetComponent<DraggableObject>());
                remainCards.RemoveAt(randomIndex);
            }
        }

        private void TeamBuildingEnd()
        {
            Debug.Log("Team building end");
        }
    }
}