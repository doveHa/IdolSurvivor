using Script.Manager;

namespace Script.ButtonClick
{
    public class ChangeSceneBeforeShow : ButtonOnClick
    {
        protected override void OnClick()
        {
            SceneLoadManager.StageEndScene();
        }
    }
}