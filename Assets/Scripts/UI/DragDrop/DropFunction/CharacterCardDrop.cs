using System.Diagnostics;
using Script.Characters;
using Script.DataDefinition.Enum;
using Script.TeamBuilding;

namespace Script.UI.DragDrop.DropFunction
{
    public class CharacterCardDrop : IDrop
    {
        void Start()
        {
            GetComponent<DraggableObject>().CanDrag = true;
        }

        public override void Click()
        {
            CharacterStats predictTeamStat =
                TeamBuildingManager.Manager.ShowAndGetPredictionTeamStat(GetComponent<CharacterCardHandler>().Character
                    .Stat);
            TeamBuildingManager.Manager.ShowTeamColor(
                TeamBuildingManager.Manager.PlayerTeam.GetTeamColor(predictTeamStat));

        }

        public override void Drop(DroppableObject drop)
        {
            transform.SetParent(drop.transform);
            TeamBuildingManager.Manager.UseCharacterCard(gameObject);
            TeamBuildingManager.Manager.AddPlayerTeam(GetComponent<CharacterCardHandler>().Character);
            TeamBuildingManager.Manager.ShowTeamStat();
            Destroy(GetComponent<DraggableObject>());
        }
    }
}