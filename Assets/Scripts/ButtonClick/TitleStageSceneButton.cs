using Script.Manager;
using Script.Stage.Event;

namespace Script.ButtonClick
{
    public class TitleStageSceneButton : ButtonOnClick
    {
        protected override void OnClick()
        {
            Config.Resource.StageData.CurrentStage = Constant.Stage.TITLE_STAGE;
            EventConfig.EventCount = 3;
            SceneLoadManager.StageScene();
        }
    }
}