using Script.Characters;
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
            TeamBuildingManager.Manager.ShowPredictionTeamStat(GetComponent<CharacterCardHandler>().Character.Stat);
        }

        public override void Drop(DroppableObject drop)
        {
            transform.SetParent(drop.transform);
            TeamBuildingManager.Manager.UseCharacterCard(gameObject);
            TeamBuildingManager.Manager.AddPlayerTeam(GetComponent<CharacterCardHandler>().Character.Stat);
            TeamBuildingManager.Manager.ShowTeamStat();
            Destroy(GetComponent<DraggableObject>());
        }
    }
}