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
                CharacterSelectManager.Manager.Player.SetName(inputField.text);
                SceneLoadManager.LoadMainScene();
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