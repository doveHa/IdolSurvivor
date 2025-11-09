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
            nameText.text = AllCharacterManager.Manager.Player.Data.name;
        }

        public void SetText(string text)
        {
            dialogueText.text = text;
        }

        public void Display()
        {
            singText.text = AllCharacterManager.Manager.Player.Stat.Sing.Value.ToString();
            danceText.text = AllCharacterManager.Manager.Player.Stat.Dance.Value.ToString();
            appearanceText.text = AllCharacterManager.Manager.Player.Stat.Appearance.Value.ToString();
            charmText.text = AllCharacterManager.Manager.Player.Stat.Charm.Value.ToString();
            voteText.text = AllCharacterManager.Manager.Player.VoteCount.ToString();
        }
    }
}