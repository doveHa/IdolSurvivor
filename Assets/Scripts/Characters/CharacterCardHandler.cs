using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Characters
{
    public class CharacterCardHandler : MonoBehaviour
    {
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI characterName, description, statVote;

        public Character Character { get; private set; }

        public void SetCharacter(Character character)
        {
            Character = character;
            SetCard();
        }

        private void SetCard()
        {
            portrait.sprite = Character.Data.portrait;
            characterName.text = Character.Data.name;
            description.text = Character.Data.description;
            statVote.text = StatVoteFormatting(Character);
        }

        private string StatVoteFormatting(Character character)
        {
            return $"{character.Stat.Sing.Value}/" +
                   $"{character.Stat.Dance.Value}/" +
                   $"{character.Stat.Appearance.Value}/" +
                   $"{character.Stat.Charm.Value}" +
                   $"{character.VoteCount}표 ({character.Rank}위)";
        }
    }
}