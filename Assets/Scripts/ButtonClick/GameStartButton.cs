using Script.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.ButtonClick
{
    public class GameStartButton : ButtonOnClick
    {
        [SerializeField] private TMP_InputField inputField;

        protected override void OnClick()
        {
            if (inputField.text.Length > 0)
            {
                AllCharacterManager.Manager.Player.Data.name = inputField.text;
                AllCharacterManager.Manager.GenerateAllCharacters();
                SceneLoadManager.GradingScene();
            }
            else
            {
                Color color = inputField.GetComponent<Outline>().effectColor;
                color.a = 1;
                inputField.GetComponent<Outline>().effectColor = color;
            }
        }
    }
}