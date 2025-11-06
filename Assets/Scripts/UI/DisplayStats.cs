using Script.Manager;
using TMPro;
using UnityEngine;

namespace Script.UI
{
    public class DisplayStats : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI nameText, singText, danceText, appearanceText, charmText, voteText, dialogueText;

        void Start()
        {
            nameText.text = CharacterSelectManager.Manager.Player.Data.name;
        }

        public void SetText(string text)
        {
            dialogueText.text = text;
        }

        public void Display()
        {
            singText.text = CharacterSelectManager.Manager.Player.Stat.Sing.Value.ToString();
            danceText.text = CharacterSelectManager.Manager.Player.Stat.Dance.Value.ToString();
            appearanceText.text = CharacterSelectManager.Manager.Player.Stat.Appearance.Value.ToString();
            charmText.text = CharacterSelectManager.Manager.Player.Stat.Charm.Value.ToString();
            voteText.text = CharacterSelectManager.Manager.Player.VoteCount.ToString();
        }
    }
}