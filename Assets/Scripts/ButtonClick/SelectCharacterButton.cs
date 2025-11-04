using Script.Manager;
using UnityEngine;

namespace Script.ButtonClick
{
    public class SelectCharacterButton : ButtonOnClick
    {
        [SerializeField] private GameObject showPanel;
        [SerializeField] private GameObject hidePanel;
        [SerializeField] private bool isFemale;


        protected override void OnClick()
        {
            showPanel.SetActive(true);
            hidePanel.SetActive(false);
            gameObject.SetActive(false);
            CharacterSelectManager.Manager.SetMode(isFemale);
        }
    }
}