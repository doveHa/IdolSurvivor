using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverBtn : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("StartScene");
    }
}
