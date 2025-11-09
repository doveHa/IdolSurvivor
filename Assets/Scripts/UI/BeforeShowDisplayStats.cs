using System.Text;
using Script.Characters;
using Script.TeamBuilding;
using TMPro;
using UnityEngine;

namespace Script.UI
{
    public class BeforeShowDisplayStats : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI[] names;
        [SerializeField] private TextMeshProUGUI[] stats;
        [SerializeField] private TextMeshProUGUI allStats;

        void Start()
        {
            for (int i = 0; i < Constant.Team.MAX_MEMBER; i++)
            {
                Character character = TeamBuildingManager.Manager.PlayerTeam.Teams[i];
                names[i].text = character.Data.name;
                stats[i].text = StatFormatting(character.Stat);
            }

            allStats.text = StatFormatting(TeamBuildingManager.Manager.PlayerTeam.TotalStat());
        }

        private string StatFormatting(CharacterStats stat)
        {
            return new StringBuilder().Append($"노래\\t\\t\\t{stat.Sing.Value}\\t춤\\t\\t\\t{stat.Dance.Value}")
                .Append($"\n외모\\t\\t\\t{stat.Appearance.Value}\\t매력\\t\\t\\t{stat.Charm.Value}").ToString();
        }
    }
}