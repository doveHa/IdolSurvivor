using UnityEngine.SceneManagement;

namespace Script.Manager
{
    public static class SceneLoadManager
    {
        public static void LoadCharacterSelectScene()
        {
        }

        public static void LoadMainScene()
        {
            SceneManager.LoadScene(Constant.Scene.MAIN);
        }
    }
}