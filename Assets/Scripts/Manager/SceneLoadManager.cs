using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Manager
{
    public class SceneLoadManager : MonoBehaviour
    {

        public static void GradingScene()
        {
            SceneManager.LoadScene(Constant.Scene.GradingScene);
        }

        public static void StageScene()
        {
            SceneManager.LoadScene(Constant.Scene.StageScene);
        }
    }
}