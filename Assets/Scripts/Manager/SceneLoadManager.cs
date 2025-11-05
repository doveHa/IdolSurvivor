using UnityEngine.SceneManagement;

namespace Script.Manager
{
    public class SceneLoadManager : ManagerBase<SceneLoadManager>
    {
        public void LoadCharacterSelectScene()
        {
        }

        public void LoadMainScene()
        {
            SceneManager.LoadScene(Constant.Scene.MAIN);
        }
    }
}