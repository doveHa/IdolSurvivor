using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Manager
{
    public class SceneLoadManager : MonoBehaviour
    {
        public static void GradingScene()
        {
            SceneManager.LoadScene(Constant.Scene.GRADING_SCENE);
        }

        public static void StageScene()
        {
            SceneManager.LoadScene(Constant.Scene.STAGE_SCENE);
        }

        public static void StageEndScene()
        {
            switch (Config.Resource.StageData.CurrentStage)
            {
                case Constant.Stage.TITLE_STAGE:
                    SceneManager.LoadScene(Constant.Scene.TEAM_BUILDING);
                    break;
            }
        }

        public static void SelectSongScene()
        {
            SceneManager.LoadScene(Constant.Scene.SELECT_SONG);
        }
    }
}