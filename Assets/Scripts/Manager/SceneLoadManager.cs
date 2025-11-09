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
            if (Config.Resource.StageData.CurrentStage.Equals(Constant.Stage.TITLE_STAGE))
            {
                SceneManager.LoadScene(Constant.Scene.TEAM_BUILDING);
            }
            else
            {
                SceneManager.LoadScene(Constant.Scene.RANKING_ANNOUNCE);
            }
        }

        public static void SelectSongScene()
        {
            SceneManager.LoadScene(Constant.Scene.SELECT_SONG);
        }
    }
}